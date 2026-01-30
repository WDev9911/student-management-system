using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.Services.Interfaces;

namespace StudentManagementSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsApiController : ControllerBase
    {
        private readonly IInstructorService _instructorService;

        public InstructorsApiController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllInstructors()
        {
            try
            {
                var instructors = await _instructorService.GetAllInstructorsAsync();
                var activeInstructors = instructors
                    .Where(i => i.Status == "Active")
                    .OrderBy(i => i.FullName)
                    .ToList();
                
                return Ok(activeInstructors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i danh sách gi?ng viên", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetInstructorById(int id)
        {
            try
            {
                var instructor = await _instructorService.GetInstructorByIdAsync(id);
                if (instructor == null)
                {
                    return NotFound(new { message = "Không tìm th?y gi?ng viên" });
                }
                return Ok(instructor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i thông tin gi?ng viên", error = ex.Message });
            }
        }
    }
}