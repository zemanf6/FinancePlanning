using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.Managers
{
    public class MonteCarloSimulator: IMonteCarloSimulator
    {
        private readonly Random _random = new();

        public SimulationResultDto Simulate(InvestmentPredictionDto input)
        {
            var finalValues = new List<decimal>();
            var sampleTrajectories = new List<List<decimal>>();
            var sampleFinals = new List<decimal>(); // pro výběr z 10 trajektorií
            var sampleSize = 10;

            var (mean, stddev) = GetParameters(input.Strategy);
            var years = input.Years;
            var monthlyContribution = input.MonthlyContribution;
            var totalMonths = years * 12;
            var ter = input.TotalExpenseRatio / 100m;

            for (int i = 0; i < input.SimulationsCount; i++)
            {
                var value = input.Principal;
                var trajectory = new List<decimal>();

                for (int month = 1; month <= totalMonths; month++)
                {
                    value += monthlyContribution;
                    var r = GetRandomReturn(mean, stddev, ter);
                    value *= (decimal)Math.Pow(1 + (double)r, 1.0 / 12.0);

                    if (month % 12 == 0)
                        trajectory.Add(value);
                }

                finalValues.Add(value);

                if (sampleTrajectories.Count < sampleSize)
                {
                    sampleTrajectories.Add(trajectory);
                    sampleFinals.Add(value);
                }
            }

            finalValues.Sort();
            var count = finalValues.Count;

            var p5 = finalValues[(int)(0.05 * count)];
            var p10 = finalValues[(int)(0.10 * count)];
            var p25 = finalValues[(int)(0.25 * count)];
            var p50 = finalValues[(int)(0.50 * count)];
            var p75 = finalValues[(int)(0.75 * count)];
            var p90 = finalValues[(int)(0.90 * count)];
            var p95 = finalValues[(int)(0.95 * count)];
            var avg = finalValues.Average();

            Dictionary<string, List<decimal>> scenarioTrajectories = new();

            if (sampleTrajectories.Count == sampleFinals.Count && sampleFinals.Count > 0)
            {
                int idxP10 = FindClosestIndex(sampleFinals, p10);
                int idxP50 = FindClosestIndex(sampleFinals, p50);
                int idxP90 = FindClosestIndex(sampleFinals, p90);

                scenarioTrajectories = new Dictionary<string, List<decimal>>
                {
                    { "percentile10", sampleTrajectories[idxP10] },
                    { "percentile50", sampleTrajectories[idxP50] },
                    { "percentile90", sampleTrajectories[idxP90] }
                };
            }

            decimal? probability = null;
            if (input.TargetAmount.HasValue)
            {
                var hits = finalValues.Count(v => v >= input.TargetAmount.Value);
                probability = Math.Round((decimal)hits / count * 100, 1);
            }

            return new SimulationResultDto
            {
                FinalValues = finalValues,
                Percentile5 = p5,
                Percentile10 = p10,
                Percentile25 = p25,
                Percentile50 = p50,
                Percentile75 = p75,
                Percentile90 = p90,
                Percentile95 = p95,
                AverageFinalValue = avg,
                TargetReachedProbability = probability,
                PercentileTrajectories = scenarioTrajectories
            };
        }


        private static (decimal mean, decimal stddev) GetParameters(InvestmentStrategy strategy)
        {
            return strategy switch
            {
                InvestmentStrategy.Conservative => (0.04m, 0.05m),
                InvestmentStrategy.Balanced => (0.06m, 0.10m),
                InvestmentStrategy.Aggressive => (0.08m, 0.15m),
                _ => (0.06m, 0.10m)
            };
        }

        private decimal GetRandomReturn(decimal mean, decimal stddev, decimal ter)
        {
            var z = NextStandardNormal();
            return mean + stddev * (decimal)z - ter;
        }

        private double NextStandardNormal()
        {
            var u1 = 1.0 - _random.NextDouble();
            var u2 = 1.0 - _random.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }
        private static int FindClosestIndex(List<decimal> sortedValues, decimal target)
        {
            decimal minDiff = decimal.MaxValue;
            int closestIndex = 0;
            for (int i = 0; i < sortedValues.Count; i++)
            {
                var diff = Math.Abs(sortedValues[i] - target);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }
    }
}
