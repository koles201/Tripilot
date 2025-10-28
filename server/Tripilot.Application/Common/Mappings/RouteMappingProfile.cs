using AutoMapper;
using Tripilot.Application.DTOs.Route;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Common.Mappings;

public class RouteMappingProfile : Profile
{
    public RouteMappingProfile()
    {
        // Route -> RouteResponse
        CreateMap<Domain.Entities.Route, RouteResponse>()
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty.ToString()))
            .ForMember(dest => dest.Privacy, opt => opt.MapFrom(src => src.Privacy.ToString()))
            .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FullName : string.Empty))
            .ForMember(dest => dest.Places, opt => opt.MapFrom(src => src.RoutePlaces.OrderBy(rp => rp.Order)));

        // RoutePlace -> RoutePlaceResponse
        CreateMap<RoutePlace, RoutePlaceResponse>()
            .ForMember(dest => dest.PlaceName, opt => opt.MapFrom(src => src.Place != null ? src.Place.Name : string.Empty))
            .ForMember(dest => dest.PlaceImageUrl, opt => opt.MapFrom(src => src.Place != null ? src.Place.ImageUrl : null))
            .ForMember(dest => dest.PlaceCity, opt => opt.MapFrom(src => src.Place != null && src.Place.Location != null ? src.Place.Location.City : string.Empty))
            .ForMember(dest => dest.PlaceCountry, opt => opt.MapFrom(src => src.Place != null && src.Place.Location != null ? src.Place.Location.Country : string.Empty));

        // Route -> RouteListResponse
        CreateMap<Domain.Entities.Route, RouteListResponse>()
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty.ToString()))
            .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FullName : string.Empty))
            .ForMember(dest => dest.PlaceCount, opt => opt.MapFrom(src => src.RoutePlaces.Count));
    }
}
