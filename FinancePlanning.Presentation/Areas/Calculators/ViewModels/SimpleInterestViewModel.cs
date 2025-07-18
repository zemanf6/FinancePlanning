﻿using FinancePlanning.Application.DTOs;
using FinancePlanning.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancePlanning.Presentation.Areas.Calculators.ViewModels
{
    public class SimpleInterestViewModel : ICalculatorViewModel
    {
        [Required]
        [Display(Name = "Principal Amount")]
        [Range(0.01, 1000000000, ErrorMessage = "Enter a valid amount")]
        public decimal Principal { get; set; }

        [Required]
        [Display(Name = "Interest Rate (%)")]
        [Range(0.01, 100, ErrorMessage = "Enter a valid rate between 0.01 and 100")]
        public decimal InterestRate { get; set; }

        [Required]
        [Display(Name = "Duration")]
        [Range(1, 120, ErrorMessage = "Enter a duration between 1 and 100")]
        public int Duration { get; set; }

        [Display(Name = "Interest Frequency")]
        public InterestFrequency? Frequency { get; set; } = InterestFrequency.Yearly;

        [Required]
        [Display(Name = "Duration Unit")]
        public PeriodUnit Unit { get; set; } = PeriodUnit.Years;

        public decimal CalculatedInterest { get; set; }
        public decimal TotalAmount { get; set; }

        public List<dynamic>? ChartData { get; set; }

        public bool ShowInYears { get; set; }
        public bool IsPartialYear { get; set; }
        public string SelectedCurrency { get; set; } = "USD";

        [MaxLength(100)]
        [Display(Name = "Note (optional)")]
        public string? Note { get; set; }

        public int? CompoundingPerYear { get => null; set { } }
    }
}