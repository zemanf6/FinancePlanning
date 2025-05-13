using FinancePlanning.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Domain.Entities
{
    public class SavedCompoundInterest
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public decimal Principal { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public PeriodUnit Unit { get; set; }

        [Required]
        public InterestFrequency Frequency { get; set; }

        [Required]
        public int CompoundingPerYear { get; set; }

        [Required]
        public string SelectedCurrency { get; set; } = "USD";

        public decimal CalculatedInterest { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Note { get; set; }
    }
}
