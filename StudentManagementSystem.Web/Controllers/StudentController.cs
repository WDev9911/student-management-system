using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.Services.Interfaces;
using System.Security.Claims;

namespace StudentManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
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

        public IActionResult Grades()
        {
            ViewData["Title"] = "My Grades";
            return View();
        }

        // Redirect to PaymentController.Wallet so students can top up
        public IActionResult Tuition()
        {
            return RedirectToAction("Wallet", "Payment");
        }

        public IActionResult Schedule()
        {
            ViewData["Title"] = "Class Schedule";
            return View();
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
    }
}