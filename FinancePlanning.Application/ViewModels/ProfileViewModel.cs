using FinancePlanning.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Application.ViewModels
{
    public class ProfileViewModel
    {
        public string Email { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Target Retirement Age")]
        public int? DefaultTargetRetirementAge { get; set; }

        [Display(Name = "Risk Profile")]
        public RiskProfile? DefaultRiskProfile { get; set; }

        [Display(Name = "Preferred Currency")]
        public string? PreferredCurrency { get; set; }
    }
}
