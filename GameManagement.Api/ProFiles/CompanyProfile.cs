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
                .ReverseMap()
                .ForMember(dest =>,
                opt =>);

        }



    }
}
