using FinancePlanning.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Infrastructure.Interfaces
{
    public interface ICompoundInterestCalculationRepository : IBaseRepository<SavedCompoundInterest>
    {
        Task<IEnumerable<SavedCompoundInterest>> GetByUserIdAsync(string userId);
        Task DeleteAllByUserIdAsync(string userId);
    }
}
