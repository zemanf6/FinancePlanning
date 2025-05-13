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
    public interface ICompoundInterestStorageManager
    {
        Task SaveCalculationAsync(CompoundInterestDto dto, ClaimsPrincipal user);
        Task<IEnumerable<SavedCompoundInterest>> GetSavedCalculationsAsync(ClaimsPrincipal user);
        Task<CompoundInterestDto?> LoadDtoByIdAsync(int id, ClaimsPrincipal user);
        Task DeleteByIdAsync(int id, ClaimsPrincipal user);
        Task DeleteAllAsync(ClaimsPrincipal user);
    }
}
