using FinancePlanning.Domain.Entities;

namespace FinancePlanning.Infrastructure.Interfaces
{
    public interface ICompoundInterestCalculationRepository
        : IBaseRepository<SavedCompoundInterest>, IUserScopedRepository<SavedCompoundInterest>
    {
    }
}
