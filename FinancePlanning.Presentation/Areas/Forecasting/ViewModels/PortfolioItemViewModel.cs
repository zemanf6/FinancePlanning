using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Presentation.Areas.Forecasting.ViewModels
{
    public class PortfolioItemViewModel
    {
        public string? AssetName { get; set; }

        [Range(-100, 100)]
        public decimal ExpectedReturn { get; set; }

        [Range(0, 100)]
        public decimal Weight { get; set; }

        [Range(0, 100)]
        public decimal StandardDeviation { get; set; }

        [Required]
        public string SelectedVolatilityLevel { get; set; } = "balanced";

    }
}
