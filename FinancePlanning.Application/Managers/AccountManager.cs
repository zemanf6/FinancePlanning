using AutoMapper;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Domain.Entities;
using FinancePlanning.Application.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;

        public AccountManager(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper)
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

        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model)
        {
            var user = mapper.Map<ApplicationUser>(model);

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, false);
                return (true, Enumerable.Empty<string>());
            }

            return (false, result.Errors.Select(e => e.Description));
        }

        public async Task<ProfileViewModel?> GetUserProfileAsync(ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user == null)
                return null;

            return mapper.Map<ProfileViewModel>(user);
        }

        public async Task<bool> UpdateUserProfileAsync(ClaimsPrincipal principal, ProfileViewModel model)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user == null)
                return false;

            mapper.Map(model, user);

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
