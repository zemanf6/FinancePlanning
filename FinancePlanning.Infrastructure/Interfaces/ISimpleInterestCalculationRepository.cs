using FinancePlanning.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Infrastructure.Interfaces
{
    public interface ISimpleInterestCalculationRepository : IBaseRepository<SavedSimpleInterest>
    {
        Task<IEnumerable<SavedSimpleInterest>> GetByUserIdAsync(string userId);
        Task DeleteAllByUserIdAsync(string userId);
    }
}
