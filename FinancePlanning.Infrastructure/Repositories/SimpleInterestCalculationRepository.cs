using FinancePlanning.Domain.Entities;
using FinancePlanning.Infrastructure.Interfaces;

namespace FinancePlanning.Infrastructure.Repositories
{
    public class SimpleInterestCalculationRepository : BaseUserScopedRepository<SavedSimpleInterest>, ISimpleInterestCalculationRepository
    {
        public SimpleInterestCalculationRepository(ApplicationDbContext context) : base(context) { }
    }
}
