using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.DTOs
{
    public class SimulationResultDto
    {
        public List<decimal> FinalValues { get; set; } = new();

        public List<List<decimal>> SampleTrajectories { get; set; } = new();

        public decimal Percentile10 { get; set; }
        public decimal Percentile50 { get; set; }
        public decimal Percentile90 { get; set; }

        public decimal AverageFinalValue { get; set; }

        public decimal? TargetReachedProbability { get; set; }

        public string? Recommendation { get; set; }
    }
}
