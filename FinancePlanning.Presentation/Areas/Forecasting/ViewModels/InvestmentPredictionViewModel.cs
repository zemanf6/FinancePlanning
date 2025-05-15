using FinancePlanning.Application.DTOs;
using FinancePlanning.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Presentation.Areas.Forecasting.ViewModels
{
    public class InvestmentPredictionViewModel
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

        [Required]
        public InvestmentStrategy Strategy { get; set; }

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

        public SimulationResultDto? Result { get; set; }
    }
}
