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
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.Longitude))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Location.Address))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Location.Country))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Location.PostalCode))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.ContactInfo.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ContactInfo.Email))
            .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.ContactInfo.Website))
            .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OperatingHours.OpenTime))
            .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.OperatingHours.CloseTime))
            .ForMember(dest => dest.DaysOfWeek, opt => opt.MapFrom(src => src.OperatingHours.DaysOfWeek))
            .ForMember(dest => dest.Is24Hours, opt => opt.MapFrom(src => src.OperatingHours.Is24Hours))
            .ForMember(dest => dest.SpecialNotes, opt => opt.MapFrom(src => src.OperatingHours.SpecialNotes))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.FullName : null));

        // Place -> PlaceListResponse
        CreateMap<Place, PlaceListResponse>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Location.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Location.Country))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()));
    }
}
