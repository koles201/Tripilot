using AutoMapper;
using MediatR;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Place;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;
using Tripilot.Domain.ValueObjects;

namespace Tripilot.Application.Features.Places.Commands;

/// <summary>
/// Handler for CreatePlaceCommand
/// </summary>
public class CreatePlaceCommandHandler : IRequestHandler<CreatePlaceCommand, PlaceResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePlaceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PlaceResponse> Handle(CreatePlaceCommand request, CancellationToken cancellationToken)
    {
        // Parse category
        if (!Enum.TryParse<PlaceCategory>(request.Category, true, out var category))
        {
            throw new ArgumentException($"Invalid category: {request.Category}");
        }

        // Create value objects
        var location = Location.Create(
            request.Latitude,
            request.Longitude,
            request.Address,
            request.City,
            request.Country,
            request.PostalCode
        );

        var contactInfo = ContactInfo.Create(
            request.Phone,
            request.Email,
            request.Website
        );

        var operatingHours = OperatingHours.Create(
            request.OpenTime,
            request.CloseTime,
            request.DaysOfWeek,
            request.Is24Hours,
            request.SpecialNotes
        );

        // Create place entity
        var place = new Place
        {
            Name = request.Name,
            Description = request.Description,
            Category = category,
            Location = location,
            ContactInfo = contactInfo,
            OperatingHours = operatingHours,
            PriceLevel = request.PriceLevel,
            Amenities = request.Amenities,
            ImageUrl = request.ImageUrl,
            GalleryImages = request.GalleryImages,
            OwnerId = request.UserRole == "BusinessOwner" ? request.UserId : null,
            IsVerified = false, // Requires admin verification
            IsActive = true,
            ViewCount = 0,
            AverageRating = 0,
            ReviewCount = 0
        };

        // Save to database
        await _unitOfWork.Repository<Place>().AddAsync(place, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to response
        var response = _mapper.Map<PlaceResponse>(place);
        return response;
    }
}
