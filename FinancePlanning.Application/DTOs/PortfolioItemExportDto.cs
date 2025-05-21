using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.DTOs
{
    public class PortfolioItemExportDto
    {
        public string? AssetName { get; set; }
        public decimal ExpectedReturn { get; set; }
        public decimal Weight { get; set; }
        public decimal StandardDeviation { get; set; }
        public string SelectedVolatilityLevel { get; set; } = "balanced";
    }

}
