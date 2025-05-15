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
                    sampleTrajectories.Add(trajectory);
            }

            finalValues.Sort();
            var count = finalValues.Count;
            var p10 = finalValues[(int)(0.1 * count)];
            var p50 = finalValues[(int)(0.5 * count)];
            var p90 = finalValues[(int)(0.9 * count)];
            var avg = finalValues.Average();

            decimal? probability = null;
            if (input.TargetAmount.HasValue)
            {
                var hits = finalValues.Count(v => v >= input.TargetAmount.Value);
                probability = Math.Round((decimal)hits / count * 100, 1);
            }

            return new SimulationResultDto
            {
                FinalValues = finalValues,
                SampleTrajectories = sampleTrajectories,
                Percentile10 = p10,
                Percentile50 = p50,
                Percentile90 = p90,
                AverageFinalValue = avg,
                TargetReachedProbability = probability,
                Recommendation = null
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
    }
}
