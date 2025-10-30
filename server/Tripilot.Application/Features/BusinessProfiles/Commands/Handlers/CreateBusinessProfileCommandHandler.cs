using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Business;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.BusinessProfiles.Commands.Handlers;

public class CreateBusinessProfileCommandHandler : IRequestHandler<CreateBusinessProfileCommand, BusinessProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBusinessProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BusinessProfileResponse> Handle(CreateBusinessProfileCommand request, CancellationToken cancellationToken)
    {
        // Ensure user exists
        var userExists = await _unitOfWork.Repository<User>()
            .GetQueryable()
            .AnyAsync(u => u.Id == request.UserId && u.IsActive, cancellationToken);
        if (!userExists)
        {
            throw new KeyNotFoundException("User not found");
        }

        // Check if profile already exists for user
        var existing = await _unitOfWork.Repository<BusinessProfile>()
            .GetQueryable()
            .FirstOrDefaultAsync(bp => bp.UserId == request.UserId, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException("Business profile already exists for this user");
        }

        var profile = new BusinessProfile
        {
            UserId = request.UserId,
            BusinessName = request.BusinessName,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            PostalCode = request.PostalCode,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
            LogoUrl = request.LogoUrl,
            BannerUrl = request.BannerUrl,
            VerificationStatus = VerificationStatus.Pending,
            ClaimedPlacesCount = 0,
            TotalViews = 0
        };

        await _unitOfWork.Repository<BusinessProfile>().AddAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<BusinessProfileResponse>(profile);
        return response;
    }
}
