using FinancePlanning.Application.DTOs;
using FinancePlanning.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Presentation.Areas.Calculators.ViewModels
{
    public class SimpleInterestViewModel
    {
        [Required]
        [Display(Name = "Principal Amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Enter a valid amount")]
        public decimal Principal { get; set; }

        [Required]
        [Display(Name = "Interest Rate (%)")]
        [Range(0.01, 100, ErrorMessage = "Enter a valid rate between 0.01 and 100")]
        public decimal InterestRate { get; set; }

        [Required]
        [Display(Name = "Duration")]
        [Range(1, 100, ErrorMessage = "Enter a duration between 1 and 100")]
        public int Duration { get; set; }

        [Display(Name = "Interest Frequency")]
        public InterestFrequency Frequency { get; set; } = InterestFrequency.Yearly;

        [Required]
        [Display(Name = "Duration Unit")]
        public PeriodUnit Unit { get; set; } = PeriodUnit.Years;

        public decimal CalculatedInterest { get; set; }
        public decimal TotalAmount { get; set; }

        public List<SimpleInterestStep>? ChartData { get; set; }

        public bool ShowInYears { get; set; }
        public bool IsPartialYear { get; set; }
        public string SelectedCurrency { get; set; } = "USD";
    }
}