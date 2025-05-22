using FinancePlanning.Application.DTOs;
using FinancePlanning.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Presentation.Areas.Forecasting.ViewModels
{
    public class InvestmentPredictionViewModel : IValidatableObject
    {
        [Required]
        [Range(0, 1000000000000)]
        public decimal Principal { get; set; }

        [Required]
        [Range(0, 1000000000)]
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

        [Range(0, 1)]
        [Display(Name = "Average Correlation")]
        public decimal Correlation { get; set; } = 0.2m;

        [Display(Name = "Inflation Rate (%)")]
        [Range(0, 100, ErrorMessage = "Inflation must be between 0% and 100%.")]
        public decimal InflationRate { get; set; } = 0.0m;


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

        [Display(Name = "Calculated Volatility")]
        public decimal CalculatedStandardDeviation
        {
            get
            {
                var activeItems = PortfolioItems
                    .Where(p => !string.IsNullOrWhiteSpace(p.AssetName) || p.Weight > 0 || p.ExpectedReturn != 0 || p.StandardDeviation > 0)
                    .ToList();

                var totalWeight = activeItems.Sum(p => p.Weight);
                if (totalWeight == 0) return 0;

                double variance = 0;

                for (int i = 0; i < activeItems.Count; i++)
                {
                    var wi = (double)activeItems[i].Weight / 100;
                    var si = (double)activeItems[i].StandardDeviation / 100;
                    variance += wi * wi * si * si;

                    for (int j = i + 1; j < activeItems.Count; j++)
                    {
                        var wj = (double)activeItems[j].Weight / 100;
                        var sj = (double)activeItems[j].StandardDeviation / 100;
                        variance += 2 * wi * wj * si * sj * (double)Correlation;
                    }
                }

                return (decimal)(Math.Sqrt(variance) * 100);
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
            if (Math.Abs(totalWeight - 100m) > 0.01m)
            {
                yield return new ValidationResult("Total portfolio weight must be 100%. (±0.01% allowed)", new[] { nameof(PortfolioItems) });
            }
        }

        public string GetBehavioralCommentary()
        {
            if (Result == null) return "";

            if (TargetAmount.HasValue && Result.TargetReachedProbability.HasValue)
            {
                var p = Result.TargetReachedProbability.Value;
                if (p < 30) return "Based on the simulation, the chance of reaching your target is very low. Consider increasing your monthly contribution or extending the investment horizon.";
                if (p < 60) return "There is moderate uncertainty in reaching your financial goal. Be cautious and review your risk tolerance and long-term plan.";
                if (p < 85) return "Your strategy seems reasonably balanced, but the market outcome may vary. Maintain consistency and avoid reacting emotionally to fluctuations.";
                return "You have a high probability of reaching your goal. Continue investing regularly, and beware of overconfidence during market highs.";
            }

            return "Simulation shows a realistic projection of future investment performance. Remember: even a good strategy needs discipline and patience over time.";
        }
    }
}
