using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlanning.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class AdminUserController : Controller
    {
        private readonly IAdminUserManager _adminUserManager;

        public AdminUserController(IAdminUserManager adminUserManager)
        {
            _adminUserManager = adminUserManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _adminUserManager.GetAllUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _adminUserManager.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            if (User.IsInRole("Admin"))
            {
                ViewBag.AllRoles = new List<string> { "Manager" };
            }
            else
            {
                ViewBag.AllRoles = new List<string>();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminUserDto user, List<string> selectedRoles)
        {
            await _adminUserManager.UpdateUserAsync(user, selectedRoles);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _adminUserManager.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
