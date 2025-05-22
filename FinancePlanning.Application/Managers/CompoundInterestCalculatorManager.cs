using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;

namespace FinancePlanning.Application.Managers
{
    public class CompoundInterestCalculatorManager : IInterestCalculatorManager<CompoundInterestDto>
    {
        private readonly IMapper _mapper;

        public CompoundInterestCalculatorManager(IMapper mapper)
        {
            _mapper = mapper;
        }

        public CompoundInterestDto Calculate(CompoundInterestDto dto)
        {
            var result = _mapper.Map<CompoundInterestDto>(dto);
            int totalPeriods = result.Unit == Domain.Enums.PeriodUnit.Years
                ? result.Duration
                : result.Duration / 12;

            result.ShowInYears = result.Unit == Domain.Enums.PeriodUnit.Years;

            try
            {
                for (int i = 1; i <= totalPeriods; i++)
                {
                    decimal compoundFactor = (decimal)Math.Pow(
                        (double)(1 + result.InterestRate / 100 / result.CompoundingPerYear),
                        result.CompoundingPerYear * i);

                    decimal total = checked(result.Principal * compoundFactor);
                    decimal interest = checked(total - result.Principal);

                    result.ChartData.Add(new InterestChartStep
                    {
                        Period = i,
                        InterestAccumulated = interest,
                        TotalAmount = total
                    });
                }

                result.CalculatedInterest = result.ChartData.Last().InterestAccumulated;
                result.TotalAmount = result.ChartData.Last().TotalAmount;
            }
            catch (Exception ex)
            {
                result.ChartData.Clear();
                result.CalculatedInterest = 0;
                result.TotalAmount = 0;
                result.ChartData.Add(new InterestChartStep
                {
                    Period = 0,
                    InterestAccumulated = 0,
                    TotalAmount = 0
                });

                throw new OverflowException("Calculation failed due to large values. Try lowering the interest rate or duration.", ex);
            }

            return result;
        }

    }
}
