using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.Services.Interfaces;

namespace StudentManagementSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesApiController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesApiController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCourses()
        {
            try
            {
                var courses = await _courseService.GetAllCoursesAsync();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i danh sách môn h?c", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCourseById(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course == null)
                {
                    return NotFound(new { message = "Không tìm th?y môn h?c" });
                }
                return Ok(course);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i thông tin môn h?c", error = ex.Message });
            }
        }

        [HttpGet("by-major/{majorId}/semester/{semesterNumber}")]
        public async Task<ActionResult> GetCoursesByMajorAndSemester(int majorId, int semesterNumber)
        {
            try
            {
                var courses = await _courseService.GetCoursesByMajorAndSemesterAsync(majorId, semesterNumber);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i danh sách môn h?c", error = ex.Message });
            }
        }
    }
}