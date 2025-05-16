using FinancePlanning.Application.DTOs;
using FinancePlanning.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Presentation.Areas.Forecasting.ViewModels
{
    public class InvestmentPredictionViewModel : IValidatableObject
    {
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Principal { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MonthlyContribution { get; set; }

        [Required]
        [Range(1, 100)]
        public int Years { get; set; }

        [Range(100, 20000)]
        public int SimulationsCount { get; set; } = 1000;

        [Display(Name = "Target Amount")]
        [Range(0, double.MaxValue)]
        public decimal? TargetAmount { get; set; }

        [Required]
        public string SelectedCurrency { get; set; } = "USD";

        [Range(0, 10)]
        [Display(Name = "Total Expense Ratio (%)")]
        public decimal TotalExpenseRatio { get; set; } = 0.0m;

        [Display(Name = "Calculated Expected Return")]
        public decimal CalculatedExpectedReturn
        {
            get
            {
                var activeItems = PortfolioItems
                    .Where(p => !string.IsNullOrWhiteSpace(p.AssetName) || p.Weight > 0 || p.ExpectedReturn != 0)
                    .ToList();

                var totalWeight = activeItems.Sum(p => p.Weight);
                if (totalWeight == 0) return 0;

                var weightedSum = activeItems.Sum(p => p.Weight * p.ExpectedReturn);
                return weightedSum / totalWeight;
            }
        }

        public SimulationResultDto? Result { get; set; }

        public List<PortfolioItemViewModel> PortfolioItems { get; set; } = new()
        {
            new()
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var activeItems = PortfolioItems
                .Where(p => !string.IsNullOrWhiteSpace(p.AssetName) || p.Weight > 0 || p.ExpectedReturn != 0)
                .ToList();

            if (activeItems.Count == 0)
            {
                yield return new ValidationResult("At least one portfolio item is required.", new[] { nameof(PortfolioItems) });
            }

            var totalWeight = activeItems.Sum(p => p.Weight);
            if (Math.Round(totalWeight) != 100)
            {
                yield return new ValidationResult("Total portfolio weight must be exactly 100%.", new[] { nameof(PortfolioItems) });
            }
        }
    }
}
