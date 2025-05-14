using FinancePlanning.Application.DTOs;

namespace FinancePlanning.Application.Interfaces
{
    public interface IAdminUserManager
    {
        Task<List<AdminUserDto>> GetAllUsersAsync();
        Task<AdminUserDto?> GetUserByIdAsync(string id);
        Task UpdateUserAsync(AdminUserDto user, List<string> selectedRoles);
        Task DeleteUserAsync(string id);
        Task<List<string>> GetAllRolesAsync();
    }
}
