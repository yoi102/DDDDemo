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
                  .ForMember(dest => dest.TitleAndPrice,
                  opt => opt.MapFrom(src => src.Title + src.Price))
                  .ForMember(dest => dest.ImageUrl,
                 opt => opt.MapFrom(src => src.DisplayItems.Select(x => x.RemoteUrl)))
                  .ReverseMap()
                  // .ForMember(dest => dest.DisplayItems.Select(x => x.RemoteUrl),
                  //opt => opt.MapFrom(src => src.DisplayItems))
                  ;
        }
    }
}
