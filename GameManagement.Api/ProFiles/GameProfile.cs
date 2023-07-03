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
                  .ForMember(dest => dest.ImageUrl,
                 opt => opt.MapFrom(src => src.ImageUrl.Select(x => x.Url)))
                  .ReverseMap()
                 // .ForMember(dest => dest.ImageUrl.Select(x => x.Url),
                 //opt => opt.MapFrom(src => src.ImageUrl))
                  ;
        }
    }
}
