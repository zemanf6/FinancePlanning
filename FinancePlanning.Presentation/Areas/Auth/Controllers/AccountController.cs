using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Presentation.Areas.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlanning.Presentation.Areas.Account.Controllers;

[Area("Auth")]
[Authorize]
public class AccountController : Controller
{
    private readonly IAccountManager accountManager;
    private readonly IMapper mapper;

    public AccountController(IAccountManager accountManager, IMapper mapper)
    {
        this.accountManager = accountManager;
        this.mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (success, error) = await accountManager.LoginAsync(model.Email, model.Password, model.RememberMe);

        if (success)
            return RedirectToAction("Index", "Home", new { area = "" });

        ModelState.AddModelError(string.Empty, error ?? "Login failed.");
        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!model.AcceptPrivacyPolicy)
            ModelState.AddModelError(nameof(model.AcceptPrivacyPolicy), "You must agree with the privacy policy.");

        if (!ModelState.IsValid)
            return View(model);

        var dto = mapper.Map<RegisterDto>(model);
        var (success, errors) = await accountManager.RegisterAsync(dto);

        if (success)
            return RedirectToAction("Index", "Home", new { area = "" });

        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await accountManager.LogoutAsync();
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var dto = await accountManager.GetUserProfileAsync(User);
        if (dto is null)
            return NotFound();

        var vm = mapper.Map<ProfileViewModel>(dto);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = mapper.Map<ProfileDto>(model);
        var success = await accountManager.UpdateUserProfileAsync(User, dto);

        if (success)
            ViewData["Message"] = "Profile updated successfully";
        else
            ModelState.AddModelError("", "Failed to update profile.");

        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await accountManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            TempData["Message"] = "If your email exists, you will receive a reset link.";
            return RedirectToAction("Login");
        }

        // For testing
        var token = await accountManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);

        TempData["Message"] = $"Password reset link: {callbackUrl}";
        //
        return RedirectToAction("Login");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        var model = new ResetPasswordViewModel { Email = email, Token = token };
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = mapper.Map<ResetPasswordDto>(model);
        var (success, errors) = await accountManager.ResetPasswordAsync(dto);

        if (success)
        {
            TempData["Message"] = "Password has been reset successfully.";
            return RedirectToAction("Login");
        }

        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        var user = await accountManager.FindByEmailAsync(User.Identity!.Name!);
        if (user == null)
            return RedirectToAction("Index", "Home", new { area = "" });

        await accountManager.DeleteUserAsync(user);

        await accountManager.LogoutAsync();
        TempData["Message"] = "Your account has been permanently deleted.";
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    [HttpGet]
    public async Task<IActionResult> DownloadProfileData()
    {
        var (fileBytes, fileName) = await accountManager.ExportUserDataAsync(User);
        return File(fileBytes, "application/json", fileName);
    }
}
