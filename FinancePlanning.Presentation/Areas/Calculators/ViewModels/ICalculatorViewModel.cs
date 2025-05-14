using FinancePlanning.Domain.Enums;

namespace FinancePlanning.Presentation.Areas.Calculators.ViewModels
{
    public interface ICalculatorViewModel
    {
        decimal Principal { get; set; }
        decimal InterestRate { get; set; }
        int Duration { get; set; }
        PeriodUnit Unit { get; set; }
        decimal CalculatedInterest { get; set; }
        decimal TotalAmount { get; set; }
        string SelectedCurrency { get; set; }
        List<object>? ChartData { get; set; }
        bool ShowInYears { get; set; }
        string? Note { get; set; }
        int? CompoundingPerYear { get; set; }
        InterestFrequency? Frequency { get; set; }
    }
}
