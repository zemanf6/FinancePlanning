using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.DTOs
{
    public class ProfileExportDto
    {
        public string Email { get; set; } = string.Empty;
        public List<SimpleInterestExportDto> SimpleInterestCalculations { get; set; } = new();
        public List<CompoundInterestExportDto> CompoundInterestCalculations { get; set; } = new();
    }

    public class SimpleInterestExportDto
    {
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public string Unit { get; set; } = "";
        public string Frequency { get; set; } = "";
        public decimal CalculatedInterest { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CompoundInterestExportDto
    {
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public string Frequency { get; set; } = "";
        public decimal FinalAmount { get; set; }
        public decimal AccumulatedInterest { get; set; }
    }
}
