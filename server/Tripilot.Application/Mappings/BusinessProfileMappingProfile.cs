using AutoMapper;
using Tripilot.Application.DTOs.Business;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Mappings;

public class BusinessProfileMappingProfile : Profile
{
    public BusinessProfileMappingProfile()
    {
        CreateMap<BusinessProfile, BusinessProfileResponse>();
        CreateMap<CreateBusinessProfileRequest, BusinessProfile>();
        CreateMap<UpdateBusinessProfileRequest, BusinessProfile>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
