using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.DTOs
{
    public class SimpleInterestDto
    {
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; }
        public PeriodUnit Unit { get; set; }
        public InterestFrequency Frequency { get; set; }

        public decimal CalculatedInterest { get; set; }
        public decimal TotalAmount { get; set; }

        public List<SimpleInterestStep> ChartData { get; set; } = new();

        public bool ShowInYears { get; set; }
        public bool IsPartialYear { get; set; }
    }

    public enum InterestFrequency
    {
        Monthly,
        Yearly
    }
    public enum PeriodUnit
    {
        Months,
        Years
    }


    public class SimpleInterestStep
    {
        public int Period { get; set; }
        public decimal InterestAccumulated { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
