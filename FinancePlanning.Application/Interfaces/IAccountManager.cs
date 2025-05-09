using FinancePlanning.Application.DTOs;
using FinancePlanning.Domain.Entities;
using System.Security.Claims;

namespace FinancePlanning.Application.Interfaces;

public interface IAccountManager
{
    Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto);
    Task<(bool Success, string? Error)> LoginAsync(string email, string password, bool rememberMe);
    Task LogoutAsync();
    Task<ProfileDto?> GetUserProfileAsync(ClaimsPrincipal principal);
    Task<bool> UpdateUserProfileAsync(ClaimsPrincipal principal, ProfileDto dto);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordDto dto);
    Task DeleteUserAsync(ApplicationUser user);
    Task<(byte[] FileBytes, string FileName)> ExportUserDataAsync(ClaimsPrincipal principal);
}