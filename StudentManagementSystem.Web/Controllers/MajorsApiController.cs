using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.Services.Interfaces;

namespace StudentManagementSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorsApiController : ControllerBase
    {
        private readonly IMajorService _majorService;

        public MajorsApiController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllMajors()
        {
            try
            {
                var majors = await _majorService.GetAllMajorsAsync();
                return Ok(majors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i danh sách chuyên ngành", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetMajorById(int id)
        {
            try
            {
                var major = await _majorService.GetMajorByIdAsync(id);
                if (major == null)
                {
                    return NotFound(new { message = "Không tìm th?y chuyên ngành" });
                }
                return Ok(major);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i khi t?i thông tin chuyên ngành", error = ex.Message });
            }
        }
    }
}