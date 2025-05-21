using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.DTOs
{
    public class InvestmentExportDto
    {
        public decimal Principal { get; set; }
        public decimal MonthlyContribution { get; set; }
        public int Years { get; set; }
        public string SelectedCurrency { get; set; } = "USD";
        public decimal ExpectedReturn { get; set; }
        public decimal StandardDeviation { get; set; }
        public SimulationResultDto? Result { get; set; }
        public List<PortfolioItemExportDto> PortfolioItems { get; set; } = new();
    }
}
