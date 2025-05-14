using FinancePlanning.Domain.Entities;

namespace FinancePlanning.Infrastructure.Interfaces
{
    public interface ISimpleInterestCalculationRepository
        : IBaseRepository<SavedSimpleInterest>, IUserScopedRepository<SavedSimpleInterest>
    {
    }
}
