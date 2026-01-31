using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.DTOs.Attendance;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.Web.ViewModels.Attendance;
using System.Security.Claims;

namespace StudentManagementSystem.Web.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly IClassService _classService;
        private readonly IInstructorService _instructorService;
        private readonly IGradeService _gradeService;
        private readonly ILogger<InstructorController> _logger;
        private readonly IAttendanceService _attendanceService;

        public InstructorController(
            IClassService classService, 
            IInstructorService instructorService,
            IGradeService gradeService,
            IAttendanceService attendanceService,
            ILogger<InstructorController> logger)
        {
            _classService = classService;
            _instructorService = instructorService;
            _gradeService = gradeService;
            _attendanceService = attendanceService;
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

        public async Task<IActionResult> Schedule(DateTime? weekStart)
        {
            ViewData["Title"] = "Teaching Schedule";

            try
            {
                var instructorIdClaim = User.FindFirst("InstructorId")?.Value;

                _logger.LogInformation($"Schedule - InstructorId claim: {instructorIdClaim}");

                if (string.IsNullOrEmpty(instructorIdClaim) || !int.TryParse(instructorIdClaim, out int instructorId))
                {
                    TempData["Error"] = "Unable to retrieve instructor information. Please contact administrator.";
                    _logger.LogWarning("Schedule - InstructorId claim not found or invalid");
                    return View(new List<StudentManagementSystem.BLL.DTOs.ClassDto>());
                }

                _logger.LogInformation($"Schedule - Fetching classes for instructor ID: {instructorId}");

                var classes = await _classService.GetClassesByInstructorAsync(instructorId);

                _logger.LogInformation($"Schedule - Found {classes.Count} classes");

                foreach (var cls in classes)
                {
                    _logger.LogInformation($"Class: {cls.ClassName}, Course: {cls.CourseName}, Schedule: {cls.Schedule}, Semester: {cls.SemesterName}");
                }

                if (classes.Count == 0)
                {
                    TempData["Info"] = "No classes have been assigned to you yet.";
                }

                // Week start (Monday) for navigation
                var baseDate = weekStart ?? DateTime.Now;
                var monday = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1);
                ViewData["WeekStart"] = monday.Date;

                return View(classes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading schedule");
                TempData["Error"] = $"Error loading schedule: {ex.Message}";
                return View(new List<StudentManagementSystem.BLL.DTOs.ClassDto>());
            }
        }

        public async Task<IActionResult> Attendance(int? classId, DateTime? date)
        {
            ViewData["Title"] = "Attendance Management";

            var instructorIdClaim = User.FindFirst("InstructorId")?.Value;
            if (string.IsNullOrEmpty(instructorIdClaim) || !int.TryParse(instructorIdClaim, out int instructorId))
            {
                TempData["Error"] = "Unable to retrieve instructor information.";
                return RedirectToAction("MyClasses");
            }

            var selectedDate = (date ?? DateTime.Today).Date;
            var classes = await _classService.GetClassesByInstructorAsync(instructorId);
            var sessions = await _attendanceService.GetInstructorSessionsAsync(instructorId);

            AttendanceSessionDto? session = null;
            if (classId.HasValue)
            {
                session = await _attendanceService.GetAttendanceSessionAsync(classId.Value, instructorId, selectedDate);
                if (session == null)
                {
                    TempData["Error"] = "No teaching session on the selected date.";
                }
            }

            return View(new InstructorAttendanceViewModel
            {
                Classes = classes,
                Sessions = sessions,
                Session = session,
                SelectedClassId = classId,
                SelectedDate = selectedDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttendance(AttendanceSaveRequest model)
        {
            var instructorIdClaim = User.FindFirst("InstructorId")?.Value;
            if (string.IsNullOrEmpty(instructorIdClaim) || !int.TryParse(instructorIdClaim, out int instructorId))
            {
                TempData["Error"] = "Unable to retrieve instructor information.";
                return RedirectToAction("MyClasses");
            }

            var success = await _attendanceService.SaveAttendanceAsync(model, instructorId);
            TempData[success ? "Success" : "Error"] = success
                ? "Attendance saved successfully."
                : "Failed to save attendance.";

            return RedirectToAction("Attendance", new { classId = model.ClassId, date = model.SessionDate.ToString("yyyy-MM-dd") });
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

        public async Task<IActionResult> ClassStudents(int classId)
        {
            ViewData["Title"] = "Class Students";

            var instructorIdClaim = User.FindFirst("InstructorId")?.Value;
            if (string.IsNullOrEmpty(instructorIdClaim) || !int.TryParse(instructorIdClaim, out int instructorId))
            {
                TempData["Error"] = "Unable to retrieve instructor information.";
                return RedirectToAction("MyClasses");
            }

            var data = await _classService.GetClassStudentsAsync(classId, instructorId);
            if (data == null)
            {
                TempData["Error"] = "Class not found or not assigned to you.";
                return RedirectToAction("MyClasses");
            }

            return View(data);
        }

        public async Task<IActionResult> ClassStudentsList()
        {
            ViewData["Title"] = "Class Students";

            var instructorIdClaim = User.FindFirst("InstructorId")?.Value;
            if (string.IsNullOrEmpty(instructorIdClaim) || !int.TryParse(instructorIdClaim, out int instructorId))
            {
                TempData["Error"] = "Unable to retrieve instructor information.";
                return RedirectToAction("MyClasses");
            }

            var classes = await _classService.GetClassesByInstructorAsync(instructorId);
            var result = new List<StudentManagementSystem.BLL.DTOs.ClassStudentsDto>();

            foreach (var cls in classes)
            {
                var data = await _classService.GetClassStudentsAsync(cls.ClassId, instructorId);
                if (data != null)
                {
                    result.Add(data);
                }
            }

            return View(result);
        }

        public async Task<IActionResult> GradeEntry(int classId)
        {
            var instructorIdClaim = User.FindFirst("InstructorId")?.Value;
            if (string.IsNullOrEmpty(instructorIdClaim) || !int.TryParse(instructorIdClaim, out int instructorId))
            {
                TempData["Error"] = "Unable to retrieve instructor information.";
                return RedirectToAction("MyClasses");
            }

            var model = await _gradeService.GetGradeEntryAsync(classId, instructorId);
            if (model == null)
            {
                TempData["Error"] = "Class not found or not assigned to you.";
                return RedirectToAction("MyClasses");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGrades(GradeEntryDto model, string action)
        {
            await _gradeService.SaveGradesAsync(model, action);

            TempData["GradeMessage"] = action switch
            {
                "publish" => "Grades have been published successfully.",
                "unpublish" => "Grades have been unpublished successfully.",
                _ => "Grades saved successfully."
            };

            return RedirectToAction("GradeEntry", new { classId = model.ClassId });
        }
    }
}