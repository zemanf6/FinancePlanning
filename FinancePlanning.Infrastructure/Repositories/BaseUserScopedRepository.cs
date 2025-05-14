using FinancePlanning.Domain.Entities;
using FinancePlanning.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancePlanning.Infrastructure.Repositories
{
    public abstract class BaseUserScopedRepository<T> : BaseRepository<T>, IUserScopedRepository<T>
        where T : class, IUserOwnedEntity
    {
        public BaseUserScopedRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<T>> GetByUserIdAsync(string userId)
            => await _dbSet.Where(e => e.UserId == userId).ToListAsync();

        public async Task DeleteAllByUserIdAsync(string userId)
        {
            var records = await _dbSet.Where(e => e.UserId == userId).ToListAsync();
            _dbSet.RemoveRange(records);
        }
    }
}
