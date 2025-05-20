using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;

namespace FinancePlanning.Application.Managers
{
    public class MonteCarloSimulator : IMonteCarloSimulator
    {
        private readonly Random _random = new();

        public SimulationResultDto Simulate(InvestmentPredictionDto input)
        {
            bool reachedMax = false;
            var finalValues = new List<decimal>();
            var sampleTrajectories = new List<List<decimal>>();
            var sampleFinals = new List<decimal>();
            var sampleSize = 10;

            var mean = Math.Clamp(input.ExpectedReturn / 100m, -1m, 1m);
            var stddev = input.StandardDeviation / 100m;
            var totalMonths = (int)(input.Years * 12);
            var monthlyContribution = input.MonthlyContribution;
            var ter = input.TotalExpenseRatio / 100m;

            for (int i = 0; i < input.SimulationsCount; i++)
            {
                decimal value = input.Principal > 0 ? input.Principal : 1m;
                var trajectory = new List<decimal>();
                double logValue = Math.Log((double)value);

                for (int month = 1; month <= totalMonths; month++)
                {
                    if (value >= decimal.MaxValue - monthlyContribution)
                    {
                        value = decimal.MaxValue;
                        reachedMax = true;
                        break;
                    }
                    else
                    {
                        value += monthlyContribution;
                    }

                    if (value <= 0) value = 1;

                    logValue = Math.Log((double)value);

                    var r = GetRandomReturn(mean, stddev, ter);
                    var monthlyReturn = Math.Exp((double)r / 12.0) - 1;

                    if (monthlyReturn > 1.0) monthlyReturn = 1.0;
                    if (monthlyReturn < -0.99) monthlyReturn = -0.99;

                    logValue += Math.Log(1.0 + monthlyReturn);

                    double expValue = Math.Exp(logValue);
                    if (double.IsInfinity(expValue) || expValue > (double)decimal.MaxValue)
                    {
                        value = decimal.MaxValue;
                        reachedMax = true;
                        break;
                    }
                    else
                    {
                        value = (decimal)expValue;
                    }

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

            decimal GetPercentile(double p) => finalValues[(int)(p * count)];
            decimal average = decimal.MaxValue;

            try
            {
                average = finalValues.Average();
            }
            catch (OverflowException)
            {
                average = decimal.MaxValue;
                reachedMax = true;
            }
            

            var result = new SimulationResultDto
            {
                FinalValues = finalValues,
                ReachedMaxValue = reachedMax,
                Percentile5 = GetPercentile(0.05),
                Percentile10 = GetPercentile(0.10),
                Percentile25 = GetPercentile(0.25),
                Percentile50 = GetPercentile(0.50),
                Percentile75 = GetPercentile(0.75),
                Percentile90 = GetPercentile(0.90),
                Percentile95 = GetPercentile(0.95),
                AverageFinalValue = average
            };

            if (input.TargetAmount.HasValue)
            {
                var hits = finalValues.Count(v => v >= input.TargetAmount.Value);
                result.TargetReachedProbability = Math.Round((decimal)hits / count * 100, 1);
            }

            if (sampleTrajectories.Count == sampleFinals.Count && sampleFinals.Count > 0)
            {
                int idxP10 = FindClosestIndex(sampleFinals, result.Percentile10);
                int idxP50 = FindClosestIndex(sampleFinals, result.Percentile50);
                int idxP90 = FindClosestIndex(sampleFinals, result.Percentile90);

                result.PercentileTrajectories = new Dictionary<string, List<decimal>>
            {
                { "percentile10", sampleTrajectories[idxP10] },
                { "percentile50", sampleTrajectories[idxP50] },
                { "percentile90", sampleTrajectories[idxP90] }
            };
            }

            return result;
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
