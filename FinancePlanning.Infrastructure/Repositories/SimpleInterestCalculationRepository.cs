using FinancePlanning.Domain.Entities;
using FinancePlanning.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Infrastructure.Repositories
{
    public class SimpleInterestCalculationRepository : BaseRepository<SavedSimpleInterest>, ISimpleInterestCalculationRepository
    {
        public SimpleInterestCalculationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<SavedSimpleInterest>> GetByUserIdAsync(string userId)
        {
            return await _dbSet.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task DeleteAllByUserIdAsync(string userId)
        {
            var records = await _dbSet.Where(x => x.UserId == userId).ToListAsync();
            _dbSet.RemoveRange(records);
        }
    }
}
