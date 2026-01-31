using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using System.Security.Claims;

namespace StudentManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRegistrationService _registrationService;
        private readonly IInstructorService _instructorService;
        private readonly ISemesterService _semesterService;

        public AdminController(
            IRegistrationService registrationService,
            IInstructorService instructorService,
            ISemesterService semesterService)
        {
            _registrationService = registrationService;
            _instructorService = instructorService;
            _semesterService = semesterService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }

        // ? GI? L?I METHOD NÀY
        public IActionResult Classes()
        {
            return Redirect("/classes.html");
        }

        public async Task<IActionResult> Semesters()
        {
            ViewData["Title"] = "Manage Semesters";
            var semesters = await _semesterService.GetAllSemestersAsync();
            return View(semesters);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSemesterDates(int semesterId, DateTime startDate, DateTime endDate)
        {
            var result = await _semesterService.UpdateSemesterDatesAsync(semesterId, startDate, endDate);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Semesters));
        }

        public async Task<IActionResult> Instructors()
        {
            ViewData["Title"] = "Manage Instructors";
            var instructors = await _instructorService.GetAllInstructorsAsync();
            return View(instructors);
        }

        [HttpGet]
        public IActionResult CreateInstructor()
        {
            ViewData["Title"] = "Create Instructor";
            return View(new CreateInstructorDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInstructor(CreateInstructorDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _instructorService.CreateInstructorAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Instructors");
        }

        [HttpGet]
        public async Task<IActionResult> EditInstructor(int id)
        {
            ViewData["Title"] = "Edit Instructor";
            var instructor = await _instructorService.GetInstructorByIdAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }

            var model = new CreateInstructorDto
            {
                FullName = instructor.FullName,
                Email = instructor.Email,
                Phone = instructor.Phone,
                Department = instructor.Department,
                Specialization = instructor.Specialization,
                Qualifications = instructor.Qualifications
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInstructor(int id, CreateInstructorDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _instructorService.UpdateInstructorAsync(id, model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Instructors");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var result = await _instructorService.DeleteInstructorAsync(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Instructors");
        }

        public async Task<IActionResult> RegistrationRequests()
        {
            ViewData["Title"] = "Registration Requests";
            var requests = await _registrationService.GetPendingRequestsAsync();
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(int requestId)
        {
            var adminUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _registrationService.ApproveRequestAsync(requestId, adminUserId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("RegistrationRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int requestId, string reason)
        {
            var adminUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _registrationService.RejectRequestAsync(requestId, adminUserId, reason);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("RegistrationRequests");
        }

        public IActionResult Students()
        {
            ViewData["Title"] = "Manage Students";
            return View();
        }

        public IActionResult Majors()
        {
            ViewData["Title"] = "Manage Majors";
            return View();
        }

        public IActionResult Courses()
        {
            ViewData["Title"] = "Manage Courses";
            return View();
        }

        public IActionResult Enrollments()
        {
            ViewData["Title"] = "Course Enrollment";
            return View();
        }

        public IActionResult Grades()
        {
            ViewData["Title"] = "Manage Grades";
            return View();
        }

        public IActionResult Tuition()
        {
            ViewData["Title"] = "Manage Tuition";
            return View();
        }

        public IActionResult Notifications()
        {
            ViewData["Title"] = "Notifications";
            return View();
        }
    }
}