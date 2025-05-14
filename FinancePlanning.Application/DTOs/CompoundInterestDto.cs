using FinancePlanning.Domain.Enums;

namespace FinancePlanning.Application.DTOs
{
    public class CompoundInterestDto
    {
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public PeriodUnit Unit { get; set; }
        public InterestFrequency Frequency { get; set; }

        public int CompoundingPerYear { get; set; }

        public decimal CalculatedInterest { get; set; }
        public decimal TotalAmount { get; set; }

        public List<InterestChartStep> ChartData { get; set; } = new();

        public bool ShowInYears { get; set; }
        public string SelectedCurrency { get; set; } = "USD";
        public string? Note { get; set; }
    }
}
