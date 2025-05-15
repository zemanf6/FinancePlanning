using FinancePlanning.Domain.Entities;
using System.Security.Claims;

namespace FinancePlanning.Application.Managers
{
    public interface IInterestStorageManager<TDto, TEntity>
        where TDto : class
        where TEntity : class, IUserOwnedEntity
    {
        Task SaveCalculationAsync(TDto dto, ClaimsPrincipal user);
        Task<IEnumerable<TEntity>> GetSavedCalculationsAsync(ClaimsPrincipal user);
        Task<TDto?> LoadDtoByIdAsync(int id, ClaimsPrincipal user);
        Task DeleteByIdAsync(int id, ClaimsPrincipal user);
        Task DeleteAllAsync(ClaimsPrincipal user);
    }
}
