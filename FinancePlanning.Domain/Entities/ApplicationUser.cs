using FinancePlanning.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace FinancePlanning.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? DefaultTargetRetirementAge { get; set; }
        public RiskProfile? DefaultRiskProfile { get; set; }
        public string? PreferredCurrency { get; set; }
    }
}
