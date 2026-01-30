using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.Services.Interfaces;

namespace StudentManagementSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemestersApiController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersApiController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllSemesters()
        {
            try
            {
                var semesters = await _semesterService.GetAllSemestersAsync();
                return Ok(semesters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i danh sách h?c k?", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetSemesterById(int id)
        {
            try
            {
                var semester = await _semesterService.GetSemesterByIdAsync(id);
                if (semester == null)
                {
                    return NotFound(new { message = "Không tìm th?y h?c k?" });
                }
                return Ok(semester);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i thông tin h?c k?", error = ex.Message });
            }
        }

        [HttpGet("current")]
        public async Task<ActionResult> GetCurrentSemester()
        {
            try
            {
                var semester = await _semesterService.GetCurrentSemesterAsync();
                if (semester == null)
                {
                    return NotFound(new { message = "Không tìm th?y h?c k? ?ang ho?t ??ng" });
                }
                return Ok(semester);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i thông tin h?c k? hi?n t?i", error = ex.Message });
            }
        }
    }
}