using AutoMapper;
using FinancePlanning.Application.ViewModels;
using FinancePlanning.Domain.Entities;
using FinancePlanning.Presentation.ViewModels;
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
            CreateMap<ApplicationUser, ProfileViewModel>().ReverseMap();

            CreateMap<RegisterViewModel, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}
