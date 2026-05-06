using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Business;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.BusinessProfiles.Commands.Handlers;

public class UpdateBusinessProfileCommandHandler : IRequestHandler<UpdateBusinessProfileCommand, BusinessProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateBusinessProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BusinessProfileResponse> Handle(UpdateBusinessProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.Repository<BusinessProfile>()
            .GetQueryable()
            .FirstOrDefaultAsync(bp => bp.Id == request.Id, cancellationToken);
        if (profile == null)
        {
            throw new KeyNotFoundException("Business profile not found");
        }
        if (profile.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this profile");
        }
        if (profile.VerificationStatus == Domain.Enums.VerificationStatus.Approved)
        {
            // Allow minor updates like description or media even after approval
        }

        // Apply changes if provided
        if (request.BusinessName != null) profile.BusinessName = request.BusinessName;
        if (request.Description != null) profile.Description = request.Description;
        if (request.Address != null) profile.Address = request.Address;
        if (request.City != null) profile.City = request.City;
        if (request.Country != null) profile.Country = request.Country;
        if (request.PostalCode != null) profile.PostalCode = request.PostalCode;
        if (request.Phone != null) profile.Phone = request.Phone;
        if (request.Email != null) profile.Email = request.Email;
        if (request.Website != null) profile.Website = request.Website;
        if (request.LogoUrl != null) profile.LogoUrl = request.LogoUrl;
        if (request.BannerUrl != null) profile.BannerUrl = request.BannerUrl;

        _unitOfWork.Repository<BusinessProfile>().Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BusinessProfileResponse>(profile);
    }
}
