using AutoMapper;
using Tripilot.Application.DTOs.Place;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Mappings;

/// <summary>
/// AutoMapper profile for Place entity mappings
/// </summary>
public class PlaceMappingProfile : Profile
{
    public PlaceMappingProfile()
    {
        // Place -> PlaceResponse
        CreateMap<Place, PlaceResponse>()
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location != null ? src.Location.Latitude : 0))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location != null ? src.Location.Longitude : 0))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Location != null ? src.Location.Address : null))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location != null ? src.Location.City : null))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Location != null ? src.Location.Country : null))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Location != null ? src.Location.PostalCode : null))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.ContactInfo != null ? src.ContactInfo.Phone : null))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ContactInfo != null ? src.ContactInfo.Email : null))
            .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.ContactInfo != null ? src.ContactInfo.Website : null))
            .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OperatingHours != null ? src.OperatingHours.OpenTime : null))
            .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.OperatingHours != null ? src.OperatingHours.CloseTime : null))
            .ForMember(dest => dest.DaysOfWeek, opt => opt.MapFrom(src => src.OperatingHours != null ? src.OperatingHours.DaysOfWeek : null))
            .ForMember(dest => dest.Is24Hours, opt => opt.MapFrom(src => src.OperatingHours != null && src.OperatingHours.Is24Hours))
            .ForMember(dest => dest.SpecialNotes, opt => opt.MapFrom(src => src.OperatingHours != null ? src.OperatingHours.SpecialNotes : null))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.FullName : null));

        // Place -> PlaceListResponse
        CreateMap<Place, PlaceListResponse>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location != null ? src.Location.City : null))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Location != null ? src.Location.Country : null))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()));
    }
}
