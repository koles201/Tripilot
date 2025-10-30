using AutoMapper;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Domain.Entities;
using System.Text.Json;

namespace Tripilot.Application.Mappings;

public class PlaceClaimMappingProfile : Profile
{
    public PlaceClaimMappingProfile()
    {
        CreateMap<PlaceClaim, PlaceClaimResponse>()
            .ForMember(dest => dest.PlaceName, opt => opt.MapFrom(src => src.Place != null ? src.Place.Name : string.Empty))
            .ForMember(dest => dest.ClaimantName, opt => opt.MapFrom(src => src.Claimant != null ? src.Claimant.FullName : string.Empty))
            .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.BusinessProfile != null ? src.BusinessProfile.BusinessName : string.Empty))
            .ForMember(dest => dest.ReviewedByAdminName, opt => opt.MapFrom(src => src.ReviewedByAdmin != null ? src.ReviewedByAdmin.FullName : null))
            .ForMember(dest => dest.DocumentUrls, opt => opt.MapFrom(src => 
                !string.IsNullOrEmpty(src.DocumentUrls) 
                    ? JsonSerializer.Deserialize<List<string>>(src.DocumentUrls, (JsonSerializerOptions?)null) 
                    : null));
    }
}
