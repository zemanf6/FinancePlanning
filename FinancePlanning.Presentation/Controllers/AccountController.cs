using FinancePlanning.Application.Interfaces;
using FinancePlanning.Application.ViewModels;
using FinancePlanning.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlanning.Presentation.Controllers
{
    [AllowAnonymous]
    public class AccountController: Controller
    {
        private readonly IAccountManager accountManager;

        public AccountController(IAccountManager accountManager)
        {
            this.accountManager = accountManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await accountManager.LoginAsync(model.Email, model.Password, model.RememberMe);

            if (success)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, error ?? "Login Failed.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, errors) = await accountManager.RegisterAsync(model);

            if (success)
                return RedirectToAction("Index", "Home");

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await accountManager.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var model = await accountManager.GetUserProfileAsync(User);
            if (model is null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await accountManager.UpdateUserProfileAsync(User, model);

            if (success)
                ViewData["Message"] = "Profile updated successfully";
            else
                ModelState.AddModelError("", "Failed to update profile.");

            return View(model);
        }
    }
}
