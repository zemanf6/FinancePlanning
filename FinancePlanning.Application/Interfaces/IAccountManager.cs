using FinancePlanning.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.Interfaces
{
    public interface IAccountManager
    {
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model);
        Task<(bool Success, string? Error)> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<ProfileViewModel?> GetUserProfileAsync(ClaimsPrincipal principal);
        Task<bool> UpdateUserProfileAsync(ClaimsPrincipal principal, ProfileViewModel model);
    }
}
