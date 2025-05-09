using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.Managers
{
    public class AdminUserManager : IAdminUserManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminUserManager(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<AdminUserDto>();

            foreach (var user in users)
            {
                var dto = _mapper.Map<AdminUserDto>(user);
                dto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
                result.Add(dto);
            }

            return result;
        }

        public async Task<AdminUserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var dto = _mapper.Map<AdminUserDto>(user);
            dto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            return dto;
        }

        public async Task UpdateUserAsync(AdminUserDto userDto, List<string> selectedRoles)
        {
            var (targetUser, targetUserRoles) = await GetTargetUserAndRoles(userDto.Id);
            if (targetUser == null) return;

            if (!await IsCurrentUserAllowedToModify(targetUser))
                return;

            var isAdmin = targetUserRoles.Contains("Admin");

            if (isAdmin)
            {
                selectedRoles = new List<string> { "Admin" };
            }
            else
            {
                var currentUser = await GetCurrentUserAsync();
                var currentRoles = await _userManager.GetRolesAsync(currentUser!);
                if (currentRoles.Contains("Admin"))
                    selectedRoles = selectedRoles.Where(r => r == "Manager").ToList();
                else
                    selectedRoles = targetUserRoles.ToList();
            }

            _mapper.Map(userDto, targetUser);
            targetUser.EmailConfirmed = true;
            targetUser.NormalizedEmail = _userManager.NormalizeEmail(targetUser.Email);
            targetUser.UserName = targetUser.Email;
            targetUser.NormalizedUserName = _userManager.NormalizeName(targetUser.Email);

            await _userManager.RemoveFromRolesAsync(targetUser, targetUserRoles);
            await _userManager.AddToRolesAsync(targetUser, selectedRoles);
            await _userManager.UpdateAsync(targetUser);
        }

        public async Task DeleteUserAsync(string id)
        {
            var (targetUser, targetUserRoles) = await GetTargetUserAndRoles(id);
            if (targetUser == null) return;

            if (!await IsCurrentUserAllowedToModify(targetUser))
                return;

            if (!targetUserRoles.Contains("Admin"))
                await _userManager.DeleteAsync(targetUser);
        }

        public Task<List<string>> GetAllRolesAsync()
            => Task.FromResult(_roleManager.Roles.Select(r => r.Name!).ToList());

        private async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return principal is null ? null : await _userManager.GetUserAsync(principal);
        }

        private async Task<bool> IsCurrentUserAllowedToModify(ApplicationUser targetUser)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var targetRoles = await _userManager.GetRolesAsync(targetUser);

            bool isCurrentAdmin = currentRoles.Contains("Admin");
            bool isTargetProtected = targetRoles.Contains("Admin") || targetRoles.Contains("Manager");

            if (!isCurrentAdmin && isTargetProtected)
                return false;

            return true;
        }

        private async Task<(ApplicationUser? User, IList<string> Roles)> GetTargetUserAndRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return (null, new List<string>());
            var roles = await _userManager.GetRolesAsync(user);
            return (user, roles);
        }
    }
}
