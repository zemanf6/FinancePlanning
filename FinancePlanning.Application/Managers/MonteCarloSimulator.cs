using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;

namespace FinancePlanning.Application.Managers
{
    public class MonteCarloSimulator : IMonteCarloSimulator
    {
        private readonly Random _random = new();

        public SimulationResultDto Simulate(InvestmentPredictionDto input)
        {
            var finalResults = new List<decimal>();
            var sampledTrajectories = new List<List<decimal>>();
            var sampledFinals = new List<decimal>();
            const int maxSamples = 10;
            bool limitReached = false;

            var meanAnnualReturn = Math.Clamp(input.ExpectedReturn / 100m, -1m, 1m);
            var stdDeviation = input.StandardDeviation / 100m;
            var months = (int)(input.Years * 12);
            var monthlyContribution = input.MonthlyContribution;
            var monthlyTER = input.TotalExpenseRatio / 100m / 12m;

            for (int i = 0; i < input.SimulationsCount; i++)
            {
                var (finalValue, trajectory, hitLimit) = RunSingleSimulation(
                    input.Principal,
                    months,
                    meanAnnualReturn,
                    stdDeviation,
                    monthlyContribution,
                    monthlyTER
                );

                finalResults.Add(finalValue);

                if (sampledTrajectories.Count < maxSamples)
                {
                    sampledTrajectories.Add(trajectory);
                    sampledFinals.Add(finalValue);
                }

                if (hitLimit)
                    limitReached = true;
            }

            finalResults.Sort();
            int total = finalResults.Count;

            decimal Percentile(double p) => finalResults[(int)(p * (total - 1))];

            var result = new SimulationResultDto
            {
                FinalValues = finalResults,
                ReachedMaxValue = limitReached,
                Percentile5 = Percentile(0.05),
                Percentile10 = Percentile(0.10),
                Percentile25 = Percentile(0.25),
                Percentile50 = Percentile(0.50),
                Percentile75 = Percentile(0.75),
                Percentile90 = Percentile(0.90),
                Percentile95 = Percentile(0.95),
                AverageFinalValue = SafeAverage(finalResults, out bool overflowed)
            };

            var realFinals = AdjustForInflation(finalResults, input.InflationRate, input.Years);

            result.RealPercentile10 = realFinals[(int)(0.10 * (realFinals.Count - 1))];
            result.RealPercentile50 = realFinals[(int)(0.50 * (realFinals.Count - 1))];
            result.RealPercentile90 = realFinals[(int)(0.90 * (realFinals.Count - 1))];
            result.RealAverageFinalValue = SafeAverage(realFinals, out _);

            if (overflowed) result.ReachedMaxValue = true;

            if (input.TargetAmount.HasValue)
            {
                var hits = finalResults.Count(v => v >= input.TargetAmount.Value);
                result.TargetReachedProbability = Math.Round((decimal)hits / total * 100, 1);
            }

            if (sampledFinals.Count > 0)
            {
                result.PercentileTrajectories = new Dictionary<string, List<decimal>>
                {
                    { "percentile10", sampledTrajectories[FindClosestIndex(sampledFinals, result.Percentile10)] },
                    { "percentile50", sampledTrajectories[FindClosestIndex(sampledFinals, result.Percentile50)] },
                    { "percentile90", sampledTrajectories[FindClosestIndex(sampledFinals, result.Percentile90)] }
                };
            }

            return result;
        }

        private (decimal finalValue, List<decimal> trajectory, bool reachedLimit) RunSingleSimulation(
            decimal principal,
            int months,
            decimal meanAnnualReturn,
            decimal stdDeviation,
            decimal monthlyContribution,
            decimal monthlyTER
        )
        {
            decimal value = principal > 0 ? principal : 1m;
            var trajectory = new List<decimal>();
            bool hitLimit = false;

            for (int month = 1; month <= months; month++)
            {
                if (value >= decimal.MaxValue - monthlyContribution)
                {
                    value = decimal.MaxValue;
                    hitLimit = true;
                    break;
                }

                value += monthlyContribution;
                if (value <= 0) value = 1;

                double logValue = Math.Log((double)value);

                var simulatedReturn = GetRandomReturn(meanAnnualReturn, stdDeviation);
                var monthlyReturn = Math.Exp((double)simulatedReturn / 12.0) - 1;

                monthlyReturn = Math.Clamp(monthlyReturn, -0.99, 1.0);

                logValue += Math.Log((1.0 + monthlyReturn) * (1.0 - (double)monthlyTER));

                double nextValue = Math.Exp(logValue);
                if (double.IsInfinity(nextValue) || nextValue > (double)decimal.MaxValue)
                {
                    value = decimal.MaxValue;
                    hitLimit = true;
                    break;
                }

                value = (decimal)nextValue;

                if (month % 12 == 0)
                    trajectory.Add(value);
            }

            return (value, trajectory, hitLimit);
        }

        private decimal GetRandomReturn(decimal mean, decimal stddev)
        {
            var z = NextStandardNormal();
            return mean + stddev * (decimal)z;
        }

        private double NextStandardNormal()
        {
            // Box-Muller transform
            var u1 = 1.0 - _random.NextDouble();
            var u2 = 1.0 - _random.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }

        private static decimal SafeAverage(List<decimal> values, out bool overflow)
        {
            try
            {
                overflow = false;
                return values.Average();
            }
            catch (OverflowException)
            {
                overflow = true;
                return decimal.MaxValue;
            }
        }

        private static int FindClosestIndex(List<decimal> values, decimal target)
        {
            decimal minDiff = decimal.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < values.Count; i++)
            {
                var diff = Math.Abs(values[i] - target);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        private static List<decimal> AdjustForInflation(List<decimal> values, decimal inflationRate, decimal years)
        {
            if (inflationRate <= 0 || years <= 0)
                return values;

            decimal inflationFactor = 1 + inflationRate / 100m;
            decimal divisor = Pow(inflationFactor, (int)years);

            return values.Select(v => v / divisor).ToList(); //
        }
        private static decimal Pow(decimal value, int exponent)
        {
            return (decimal)Math.Pow((double)value, exponent);
        }
    }
}
