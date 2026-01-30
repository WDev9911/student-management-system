using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;

namespace StudentManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IStudentService _studentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IVnPayService vnPayService,
            IStudentService studentService,
            ILogger<PaymentController> logger)
        {
            _vnPayService = vnPayService;
            _studentService = studentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Wallet()
        {
            var studentIdClaim = User.FindFirst("StudentId")?.Value;
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = await _studentService.GetWalletInfoAsync(studentId);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult CreatePayment([FromBody] VnPayPaymentRequest request)
        {
            try
            {
                var studentIdClaim = User.FindFirst("StudentId")?.Value;
                if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out int studentId))
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                request.StudentId = studentId;
                request.Description = $"Nap tien vi - {request.Amount:N0} VND";

                // Pass client IP string to BLL service (IVnPayService uses string clientIp now)
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                var paymentUrl = _vnPayService.CreatePaymentUrl(request, clientIp);

                return Json(new { success = true, paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Return()
        {
            try
            {
                var queryDict = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
                var callback = _vnPayService.ProcessCallback(queryDict);

                _logger.LogInformation($"Payment callback: TxnRef={callback.vnp_TxnRef}, ResponseCode={callback.vnp_ResponseCode}");

                var success = await _vnPayService.ProcessPaymentAsync(callback);

                // Pass a user-friendly message to the next page via TempData
                if (success)
                {
                    TempData["PaymentResult"] = "N?p ti?n thành công! S? d? ?ã ???c c?p nh?t.";
                    TempData["PaymentAmount"] = (long.Parse(callback.vnp_Amount) / 100).ToString();
                }
                else
                {
                    TempData["PaymentResult"] = "Giao d?ch th?t b?i ho?c ?ã b? h?y.";
                }

                // Redirect to student dashboard (Home)
                return RedirectToAction("Index", "Student");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment return");
                TempData["PaymentResult"] = "Có l?i x?y ra khi x? lý giao d?ch.";
                return RedirectToAction("Index", "Student");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IPN()
        {
            try
            {
                var queryDict = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
                var callback = _vnPayService.ProcessCallback(queryDict);

                _logger.LogInformation($"IPN callback: TxnRef={callback.vnp_TxnRef}, ResponseCode={callback.vnp_ResponseCode}");

                var success = await _vnPayService.ProcessPaymentAsync(callback);

                if (success)
                {
                    return Json(new { RspCode = "00", Message = "Confirm Success" });
                }
                else
                {
                    return Json(new { RspCode = "99", Message = "Unknown error" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing IPN");
                return Json(new { RspCode = "99", Message = ex.Message });
            }
        }
    }
}