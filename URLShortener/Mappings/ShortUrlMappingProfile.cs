using AutoMapper;
using URLShortener.DTOs;
using URLShortener.InputModels;
using URLShortener.Models;

namespace URLShortener.Mappings
{
    public class ShortUrlMappingProfile : Profile
    {
        public ShortUrlMappingProfile()
        {
            CreateMap<ShortUrl, ShortUrlTableDto>();

            CreateMap<ShortUrl, ShortUrlDetailsDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.User.Username));

            CreateMap<CreateShortUrlRequest, ShortUrl>();
        }
    }
}
