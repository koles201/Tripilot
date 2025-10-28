using AutoMapper;
using Tripilot.Application.DTOs.Review;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Common.Mappings;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        // Review -> ReviewResponse
        CreateMap<Review, ReviewResponse>()
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.FullName : string.Empty))
            .ForMember(dest => dest.PlaceName, opt => opt.MapFrom(src => src.Place != null ? src.Place.Name : null))
            .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.Route != null ? src.Route.Name : null));
    }
}
