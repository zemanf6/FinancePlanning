using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Domain.Entities;
using FinancePlanning.Presentation.Areas.Auth.ViewModels;
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
        }
    }
}
