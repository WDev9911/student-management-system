using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;

namespace StudentManagementSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ClassesApiController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassesApiController(IClassService classService)
        {
            _classService = classService;
        }           

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var classes = await _classService.GetAllClassesAsync();
                return Ok(classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i t?i l?p", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var cls = await _classService.GetClassByIdAsync(id);
                if (cls == null) return NotFound(new { message = "Không tìm th?y" });
                return Ok(cls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i", error = ex.Message });
            }
        }

        [HttpGet("by-course/{courseId}/semester/{semesterId}")]
        public async Task<IActionResult> GetByCourseAndSemester(int courseId, int semesterId)
        {
            try
            {
                var classes = await _classService.GetClassesByCourseAndSemesterAsync(courseId, semesterId);
                return Ok(classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClassRequest request)
        {
            try
            {
                var result = await _classService.CreateClassAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.ClassId }, result);
            }
            catch (FormatException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i t?o l?p", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClassRequest request)
        {
            try
            {
                var result = await _classService.UpdateClassAsync(id, request);
                return Ok(result);
            }
            catch (FormatException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i c?p nh?t", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _classService.DeleteClassAsync(id);
                if (!result) return NotFound(new { message = "Không tìm th?y" });
                return Ok(new { message = "Xóa thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i xóa", error = ex.Message });
            }
        }

        [HttpPost("check-conflicts")]
        public async Task<IActionResult> CheckConflicts([FromBody] ConflictCheckRequest request)
        {
            try
            {
                var result = await _classService.CheckConflictsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "L?i ki?m tra", error = ex.Message });
            }
        }
    }
}