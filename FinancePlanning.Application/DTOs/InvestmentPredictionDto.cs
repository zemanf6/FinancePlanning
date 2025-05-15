using FinancePlanning.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.DTOs
{
    public class InvestmentPredictionDto
    {
        public decimal Principal { get; set; }
        public decimal MonthlyContribution { get; set; }
        public decimal Years { get; set; }
        public InvestmentStrategy Strategy { get; set; }
        public int SimulationsCount { get; set; } = 1000;
        public decimal? TargetAmount { get; set; }
        public string SelectedCurrency { get; set; } = "USD";

        public decimal TotalExpenseRatio { get; set; } = 0.0m;
    }
}
