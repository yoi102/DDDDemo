using AutoMapper;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Models;

namespace GameManagement.Api.ProFiles
{
    public class PublisherProfile : MapperConfigurationExpression
    {
        public PublisherProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ReverseMap()
                .ForMember(dest =>,
                opt =>);

        }



    }
}
