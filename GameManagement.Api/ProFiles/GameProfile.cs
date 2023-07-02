using AutoMapper;
using GameManagement.Shared.Entities;
using GameManagement.Shared.Models;

namespace GameManagement.Api.ProFiles
{
    public class GameProfile : MapperConfigurationExpression
    {
        public GameProfile()
        {
            CreateMap<Game, GameDto>()
                 .ReverseMap()
                 .ForMember(dest =>,
                 opt =>);
        }
    }
}
