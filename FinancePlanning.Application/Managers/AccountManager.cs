using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FinancePlanning.Application.Managers;

public class AccountManager : IAccountManager
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IMapper mapper;

    public AccountManager(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          IMapper mapper)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.mapper = mapper;
    }

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password, bool rememberMe)
    {
        var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, false);
        return result.Succeeded ? (true, null) : (false, "Invalid login attempt.");
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto)
    {
        var user = mapper.Map<ApplicationUser>(dto);
        var result = await userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, false);
            return (true, Enumerable.Empty<string>());
        }

        return (false, result.Errors.Select(e => e.Description));
    }

    public async Task<ProfileDto?> GetUserProfileAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return null;

        return mapper.Map<ProfileDto>(user);
    }

    public async Task<bool> UpdateUserProfileAsync(ClaimsPrincipal principal, ProfileDto dto)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return false;

        mapper.Map(dto, user);

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
    {
        return await userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return (false, new[] { "User not found." });

        var result = await userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        return result.Succeeded
            ? (true, Enumerable.Empty<string>())
            : (false, result.Errors.Select(e => e.Description));
    }
}
