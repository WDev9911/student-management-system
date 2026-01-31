using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.DTOs.Attendance;
using StudentManagementSystem.BLL.DTOs.CourseRegistration;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.Web.ViewModels.Attendance;
using System.Security.Claims;

namespace StudentManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IAttendanceService _attendanceService;

        public StudentController(
            IStudentService studentService, 
            IEnrollmentService enrollmentService,
            IAttendanceService attendanceService)
        {
            _studentService = studentService;
            _enrollmentService = enrollmentService;
            _attendanceService = attendanceService;
        }

        public async Task<IActionResult> Index()
        {
            var studentIdClaim = User.FindFirstValue("StudentId");
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out var studentId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var dashboardData = await _studentService.GetDashboardDataAsync(studentId);
            if (dashboardData == null)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            ViewData["Title"] = $"Dashboard - {dashboardData.FullName} ({dashboardData.StudentCode})";
            return View(dashboardData);
        }

        public IActionResult Enrollment()
        {
            ViewData["Title"] = "Course Enrollment";
            return View();
        }

        public async Task<IActionResult> Grades()
        {
            var studentIdClaim = User.FindFirstValue("StudentId");
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out var studentId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var grades = await _studentService.GetPublishedGradesAsync(studentId);
            ViewData["Title"] = "My Grades";
            return View(grades);
        }

        // Redirect to PaymentController.Wallet so students can top up
        public IActionResult Tuition()
        {
            return RedirectToAction("Wallet", "Payment");
        }

        public async Task<IActionResult> Schedule()
        {
            var studentIdClaim = User.FindFirstValue("StudentId");
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out var studentId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var scheduleData = await _studentService.GetStudentScheduleAsync(studentId);
            ViewData["Title"] = "My Schedule";
            return View(scheduleData);
        }

        public IActionResult Profile()
        {
            ViewData["Title"] = "My Profile";
            return View();
        }

        public IActionResult Notifications()
        {
            ViewData["Title"] = "Notifications";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CourseRegistration(int? retakeCourseId)
        {
            try
            {
                var studentIdClaim = User.FindFirst("StudentId")?.Value;
                if (string.IsNullOrEmpty(studentIdClaim))
                {
                    return RedirectToAction("Login", "Auth");
                }

                int studentId = int.Parse(studentIdClaim);

                var walletInfo = await _studentService.GetWalletInfoAsync(studentId);
                ViewBag.WalletBalance = walletInfo?.Balance.ToString("N0") ?? "0";
                ViewBag.RetakeCourseId = retakeCourseId ?? 0;

                var data = await _enrollmentService.GetCourseRegistrationDataAsync(studentId);

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEnrollment([FromBody] EnrollmentRequestDto request)
        {
            try
            {
                var studentIdClaim = User.FindFirst("StudentId")?.Value;
                if (string.IsNullOrEmpty(studentIdClaim))
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                request.StudentId = int.Parse(studentIdClaim);
                var result = await _enrollmentService.EnrollStudentAsync(request);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    totalFee = result.TotalFee,
                    remainingBalance = result.RemainingBalance,
                    enrolledCourses = result.EnrolledCourses,
                    errors = result.Errors
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> PendingCourses()
        {
            var studentIdClaim = User.FindFirstValue("StudentId");
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out var studentId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var pending = await _enrollmentService.GetPendingCoursesAsync(studentId);
            ViewData["Title"] = "Pending Courses";
            return View(pending);
        }

        [HttpGet]
        public async Task<IActionResult> RetakeRegistration()
        {
            try
            {
                var studentIdClaim = User.FindFirst("StudentId")?.Value;
                if (string.IsNullOrEmpty(studentIdClaim))
                {
                    return RedirectToAction("Login", "Auth");
                }

                int studentId = int.Parse(studentIdClaim);

                var walletInfo = await _studentService.GetWalletInfoAsync(studentId);
                ViewBag.WalletBalance = walletInfo?.Balance.ToString("N0") ?? "0";

                var data = await _enrollmentService.GetRetakeRegistrationDataAsync(studentId);

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Attendance(DateTime? date, int? classId)
        {
            var studentIdClaim = User.FindFirstValue("StudentId");
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out var studentId))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var selectedDate = (date ?? DateTime.Today).Date;

            var classes = await _attendanceService.GetStudentClassesAsync(studentId);
            var items = await _attendanceService.GetStudentAttendanceAsync(studentId, selectedDate, selectedDate);

            if (classId.HasValue)
            {
                items = items.Where(i => i.ClassId == classId.Value).ToList();
            }

            ViewData["Title"] = "My Attendance";
            return View(new StudentAttendanceViewModel
            {
                SelectedDate = selectedDate,
                SelectedClassId = classId,
                Classes = classes,
                Items = items
            });
        }
    }
}