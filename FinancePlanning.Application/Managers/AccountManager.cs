using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace FinancePlanning.Application.Managers;

public class AccountManager : IAccountManager
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;

    public AccountManager(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password, bool rememberMe)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);
        return result.Succeeded ? (true, null) : (false, "Invalid login attempt.");
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto)
    {
        var user = _mapper.Map<ApplicationUser>(dto);
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return (true, Enumerable.Empty<string>());
        }

        return (false, result.Errors.Select(e => e.Description));
    }

    public async Task<ProfileDto?> GetUserProfileAsync(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal);
        if (user == null) return null;

        return _mapper.Map<ProfileDto>(user);
    }

    public async Task<bool> UpdateUserProfileAsync(ClaimsPrincipal principal, ProfileDto dto)
    {
        var user = await _userManager.GetUserAsync(principal);
        if (user == null) return false;

        _mapper.Map(dto, user);

        user.EmailConfirmed = true;
        user.NormalizedEmail = _userManager.NormalizeEmail(user.Email);
        user.UserName = user.Email;
        user.NormalizedUserName = _userManager.NormalizeName(user.Email);

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return (false, new[] { "User not found." });

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        return result.Succeeded
            ? (true, Enumerable.Empty<string>())
            : (false, result.Errors.Select(e => e.Description));
    }

    public async Task DeleteUserAsync(ApplicationUser user)
    {
        await _userManager.DeleteAsync(user);
    }

    public async Task<(byte[] FileBytes, string FileName)> ExportUserDataAsync(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var dto = _mapper.Map<ProfileDto>(user);

        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var fileName = $"profile-data-{user.Email}.json";
        var fileBytes = Encoding.UTF8.GetBytes(json);

        return (fileBytes, fileName);
    }
}
