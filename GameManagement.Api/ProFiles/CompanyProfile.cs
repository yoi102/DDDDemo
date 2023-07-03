using AutoMapper;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Models;

namespace GameManagement.Api.ProFiles
{
    public class CompanyProfile : MapperConfigurationExpression
    {
        public CompanyProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.CompanyName,
                opt => opt.MapFrom(src => src.Name))
                .ReverseMap()
                ;

        }



    }
}
