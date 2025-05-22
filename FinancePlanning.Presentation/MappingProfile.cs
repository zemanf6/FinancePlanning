using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Domain.Entities;
using FinancePlanning.Presentation.Areas.Auth.ViewModels;
using FinancePlanning.Presentation.Areas.Calculators.ViewModels;
using FinancePlanning.Presentation.Areas.Forecasting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ProfileDto>().ReverseMap();

            CreateMap<ProfileDto, ProfileViewModel>().ReverseMap();

            CreateMap<RegisterViewModel, RegisterDto>();

            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<ResetPasswordViewModel, ResetPasswordDto>();
            CreateMap<ApplicationUser, AdminUserDto>().ReverseMap();
            CreateMap<SimpleInterestViewModel, SimpleInterestDto>().ReverseMap();
            CreateMap<SavedSimpleInterest, SimpleInterestDto>().ReverseMap();
            CreateMap<CompoundInterestViewModel, CompoundInterestDto>().ReverseMap();
            CreateMap<SavedCompoundInterest, CompoundInterestDto>().ReverseMap();
            CreateMap<InvestmentPredictionViewModel, InvestmentPredictionDto>()
                .ForMember(dest => dest.ExpectedReturn, opt => opt.MapFrom(src => src.CalculatedExpectedReturn))
                .ForMember(dest => dest.StandardDeviation, opt => opt.MapFrom(src => src.CalculatedStandardDeviation));

            CreateMap<InvestmentPredictionViewModel, InvestmentExportDto>()
                .ForMember(dest => dest.ExpectedReturn, opt => opt.Ignore())
                .ForMember(dest => dest.StandardDeviation, opt => opt.Ignore())
                .ForMember(dest => dest.Result, opt => opt.Ignore());
            CreateMap<PortfolioItemViewModel, PortfolioItemExportDto>();
            CreateMap<InvestmentPredictionViewModel, InvestmentExportDto>();
            CreateMap<SavedCompoundInterest, CompoundInterestExportDto>();
            CreateMap<SavedSimpleInterest, SimpleInterestExportDto>();
        }
    }
}
