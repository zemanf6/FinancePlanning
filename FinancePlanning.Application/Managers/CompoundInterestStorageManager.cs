using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Domain.Entities;
using FinancePlanning.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FinancePlanning.Application.Managers
{
    public class CompoundInterestStorageManager
        : BaseInterestStorageManager<CompoundInterestDto, SavedCompoundInterest, ICompoundInterestCalculationRepository>,
          ICompoundInterestStorageManager
    {
        public CompoundInterestStorageManager(
            ICompoundInterestCalculationRepository repository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
            : base(repository, userManager, mapper) { }
    }
}
