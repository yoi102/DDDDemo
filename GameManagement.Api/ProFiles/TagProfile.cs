using AutoMapper;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Models;

namespace GameManagement.Api.ProFiles
{
    public class TagProfile : MapperConfigurationExpression
    {

        public TagProfile()
        {
            CreateMap<Tag, AddTagDto>().ReverseMap();
        }

    }
}
