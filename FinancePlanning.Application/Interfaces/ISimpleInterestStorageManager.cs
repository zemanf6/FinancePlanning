using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Managers;
using FinancePlanning.Domain.Entities;

namespace FinancePlanning.Application.Interfaces
{
    public interface ISimpleInterestStorageManager
        : IInterestStorageManager<SimpleInterestDto, SavedSimpleInterest>
    {
    }
}
