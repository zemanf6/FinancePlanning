using FinancePlanning.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.DTOs
{
    public class ProfileDto
    {
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? DefaultTargetRetirementAge { get; set; }
        public RiskProfile? DefaultRiskProfile { get; set; }
        public string? PreferredCurrency { get; set; }
    }
}
