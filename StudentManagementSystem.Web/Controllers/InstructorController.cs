using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.Services.Interfaces;
using System.Security.Claims;

namespace StudentManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly IClassService _classService;
        private readonly IInstructorService _instructorService;
        private readonly ILogger<InstructorController> _logger;

        public InstructorController(
            IClassService classService, 
            IInstructorService instructorService,
            ILogger<InstructorController> logger)
        {
            _classService = classService;
            _instructorService = instructorService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }

        public async Task<IActionResult> MyClasses()
        {
            ViewData["Title"] = "My Classes";
            
            try
            {
                // Get instructor ID from claims
                var instructorIdClaim = User.FindFirst("InstructorId")?.Value;
                
                _logger.LogInformation($"InstructorId claim: {instructorIdClaim}");
                _logger.LogInformation($"All claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
                
                if (string.IsNullOrEmpty(instructorIdClaim) || !int.TryParse(instructorIdClaim, out int instructorId))
                {
                    TempData["Error"] = "Unable to retrieve instructor information. Please contact administrator.";
                    _logger.LogWarning("InstructorId claim not found or invalid");
                    return View(new List<StudentManagementSystem.BLL.DTOs.ClassDto>());
                }

                _logger.LogInformation($"Fetching classes for instructor ID: {instructorId}");
                
                // Get classes assigned to this instructor
                var classes = await _classService.GetClassesByInstructorAsync(instructorId);
                
                _logger.LogInformation($"Found {classes.Count} classes for instructor {instructorId}");
                
                if (classes.Count == 0)
                {
                    TempData["Info"] = "No classes have been assigned to you yet. Please contact the administrator if this is incorrect.";
                }
                
                return View(classes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading classes");
                TempData["Error"] = $"Error loading classes: {ex.Message}";
                return View(new List<StudentManagementSystem.BLL.DTOs.ClassDto>());
            }
        }

        public async Task<IActionResult> Schedule()
        {
            ViewData["Title"] = "Teaching Schedule";
            
            try
            {
                // Get instructor ID from claims
                var instructorIdClaim = User.FindFirst("InstructorId")?.Value;
                
                _logger.LogInformation($"Schedule - InstructorId claim: {instructorIdClaim}");
                
                if (string.IsNullOrEmpty(instructorIdClaim) || !int.TryParse(instructorIdClaim, out int instructorId))
                {
                    TempData["Error"] = "Unable to retrieve instructor information. Please contact administrator.";
                    _logger.LogWarning("Schedule - InstructorId claim not found or invalid");
                    return View(new List<StudentManagementSystem.BLL.DTOs.ClassDto>());
                }

                _logger.LogInformation($"Schedule - Fetching classes for instructor ID: {instructorId}");
                
                // Get classes assigned to this instructor
                var classes = await _classService.GetClassesByInstructorAsync(instructorId);
                
                _logger.LogInformation($"Schedule - Found {classes.Count} classes");
                
                // Log detailed class information
                foreach (var cls in classes)
                {
                    _logger.LogInformation($"Class: {cls.ClassName}, Course: {cls.CourseName}, Schedule: {cls.Schedule}, Semester: {cls.SemesterName}");
                }
                
                if (classes.Count == 0)
                {
                    TempData["Info"] = "No classes have been assigned to you yet.";
                }
                
                return View(classes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading schedule");
                TempData["Error"] = $"Error loading schedule: {ex.Message}";
                return View(new List<StudentManagementSystem.BLL.DTOs.ClassDto>());
            }
        }

        public IActionResult Attendance()
        {
            ViewData["Title"] = "Attendance Management";
            return View();
        }

        public IActionResult Grades()
        {
            ViewData["Title"] = "Grade Management";
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