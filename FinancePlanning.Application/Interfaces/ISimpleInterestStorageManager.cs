using FinancePlanning.Application.DTOs;
using FinancePlanning.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.Interfaces
{
    public interface ISimpleInterestStorageManager
    {
        Task SaveCalculationAsync(SimpleInterestDto dto, ClaimsPrincipal user);
        Task<IEnumerable<SavedSimpleInterest>> GetSavedCalculationsAsync(ClaimsPrincipal user);
        Task<SimpleInterestDto?> LoadDtoByIdAsync(int id, ClaimsPrincipal user);
        Task DeleteByIdAsync(int id, ClaimsPrincipal user);
        Task DeleteAllAsync(ClaimsPrincipal user);
    }
}
