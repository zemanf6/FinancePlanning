using FinancePlanning.Domain.Entities;
using FinancePlanning.Infrastructure.Interfaces;

namespace FinancePlanning.Infrastructure.Repositories
{
    public class CompoundInterestCalculationRepository : BaseUserScopedRepository<SavedCompoundInterest>, ICompoundInterestCalculationRepository
    {
        public CompoundInterestCalculationRepository(ApplicationDbContext context) : base(context) { }
    }
}
