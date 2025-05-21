using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FinancePlanning.Application.DTOs
{
    public class SimulationResultDto
    {
        public List<decimal> FinalValues { get; set; } = new();

        [XmlIgnore]
        public Dictionary<string, List<decimal>>? PercentileTrajectories { get; set; }

        public decimal Percentile5 { get; set; }
        public decimal Percentile10 { get; set; }
        public decimal Percentile25 { get; set; }
        public decimal Percentile50 { get; set; }
        public decimal Percentile75 { get; set; }
        public decimal Percentile90 { get; set; }
        public decimal Percentile95 { get; set; }

        public decimal AverageFinalValue { get; set; }

        public decimal? TargetReachedProbability { get; set; }

        public string? Recommendation { get; set; }
        public bool ReachedMaxValue { get; set; }
    }

}
