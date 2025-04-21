using FinancePlanning.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Application.ViewModels
{
    public class ProfileViewModel: IValidatableObject
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var today = DateTime.Today;
            var minDate = new DateTime(1900, 1, 1);

            if (BirthDate.HasValue)
            {
                if (BirthDate.Value < minDate || BirthDate.Value > today)
                {
                    yield return new ValidationResult(
                        "Birth date must be between 1.1.1900 and today.",
                        new[] { nameof(BirthDate) });
                }

                if (DefaultTargetRetirementAge.HasValue)
                {
                    var currentAge = (int)((today - BirthDate.Value).TotalDays / 365.25);
                    if (DefaultTargetRetirementAge <= currentAge)
                    {
                        yield return new ValidationResult(
                            "Target retirement age must be greater than your current age.",
                            new[] { nameof(DefaultTargetRetirementAge) });
                    }
                }
            }
        }

    }
}
