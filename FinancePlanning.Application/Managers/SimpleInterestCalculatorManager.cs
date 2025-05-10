using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.Managers
{
    public class SimpleInterestCalculatorManager : ISimpleInterestCalculatorManager
    {
        private readonly IMapper mapper;

        public SimpleInterestCalculatorManager(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public SimpleInterestDto Calculate(SimpleInterestDto model)
        {
            var dto = mapper.Map<SimpleInterestDto>(model);
            InitializeCalculationMetadata(dto);

            if (dto.ShowInYears)
                CalculateYearly(dto);
            else
                CalculateMonthly(dto);

            dto.CalculatedInterest = dto.ChartData.Last().InterestAccumulated;
            dto.TotalAmount = dto.ChartData.Last().TotalAmount;
            return dto;
        }

        private static void InitializeCalculationMetadata(SimpleInterestDto dto)
        {
            int totalMonths = dto.Unit == PeriodUnit.Years ? dto.Duration * 12 : dto.Duration;

            bool isWholeYears = totalMonths % 12 == 0;
            dto.ShowInYears = dto.Frequency == InterestFrequency.Yearly && isWholeYears;
            dto.IsPartialYear = dto.Frequency == InterestFrequency.Yearly && !isWholeYears;
        }

        private static void CalculateYearly(SimpleInterestDto dto)
        {
            int totalYears = dto.Unit == PeriodUnit.Years ? dto.Duration : dto.Duration / 12;
            for (int year = 1; year <= totalYears; year++)
            {
                decimal interest = dto.Principal * dto.InterestRate / 100 * year;
                decimal total = dto.Principal + interest;

                dto.ChartData.Add(new SimpleInterestStep
                {
                    Period = year,
                    InterestAccumulated = interest,
                    TotalAmount = total
                });
            }
        }

        private static void CalculateMonthly(SimpleInterestDto dto)
        {
            int totalMonths = dto.Unit == PeriodUnit.Years ? dto.Duration * 12 : dto.Duration;
            decimal ratePerMonth = (dto.Frequency == InterestFrequency.Monthly)
                ? dto.InterestRate / 100
                : dto.InterestRate / 100 / 12;

            for (int month = 1; month <= totalMonths; month++)
            {
                decimal interest = dto.Principal * ratePerMonth * month;
                decimal total = dto.Principal + interest;

                dto.ChartData.Add(new SimpleInterestStep
                {
                    Period = month,
                    InterestAccumulated = interest,
                    TotalAmount = total
                });
            }
        }
    }
}
