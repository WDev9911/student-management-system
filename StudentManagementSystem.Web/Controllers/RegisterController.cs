using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using System.Linq;

namespace StudentManagementSystem.Web.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IRegistrationService _registrationService;

        public RegisterController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var majors = await _registrationService.GetMajorsAsync();
            ViewBag.Majors = majors.Select(m => new SelectListItem
            {
                Value = m.MajorId.ToString(),
                Text = m.MajorName
            }).ToList();
            return View(new RegisterRequestDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                var majors = await _registrationService.GetMajorsAsync();
                ViewBag.Majors = majors.Select(m => new SelectListItem
                {
                    Value = m.MajorId.ToString(),
                    Text = m.MajorName
                }).ToList();
                return View(model);
            }

            var result = await _registrationService.RegisterAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                var majors = await _registrationService.GetMajorsAsync();
                ViewBag.Majors = majors.Select(m => new SelectListItem
                {
                    Value = m.MajorId.ToString(),
                    Text = m.MajorName
                }).ToList();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }
    }
}