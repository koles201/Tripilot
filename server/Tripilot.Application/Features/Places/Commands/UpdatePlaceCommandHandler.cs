using AutoMapper;
using MediatR;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Place;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;
using Tripilot.Domain.ValueObjects;

namespace Tripilot.Application.Features.Places.Commands;

/// <summary>
/// Handler for UpdatePlaceCommand
/// </summary>
public class UpdatePlaceCommandHandler : IRequestHandler<UpdatePlaceCommand, PlaceResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdatePlaceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PlaceResponse> Handle(UpdatePlaceCommand request, CancellationToken cancellationToken)
    {
        // Get existing place
        var place = await _unitOfWork.Repository<Place>().GetByIdAsync(request.PlaceId, cancellationToken);
        
        if (place == null)
        {
            throw new KeyNotFoundException($"Place with ID {request.PlaceId} not found");
        }

        // Authorization check: Only owner or admin can update
        if (request.UserRole != "Admin" && place.OwnerId != request.UserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this place");
        }

        // Parse category
        if (!Enum.TryParse<PlaceCategory>(request.Category, true, out var category))
        {
            throw new ArgumentException($"Invalid category: {request.Category}");
        }

        // Update value objects
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

        // Update place properties
        place.Name = request.Name;
        place.Description = request.Description;
        place.Category = category;
        place.Location = location;
        place.ContactInfo = contactInfo;
        place.OperatingHours = operatingHours;
        place.PriceLevel = request.PriceLevel;
        place.Amenities = request.Amenities;
        place.ImageUrl = request.ImageUrl;
        place.GalleryImages = request.GalleryImages;
        place.IsActive = request.IsActive;

        // Save changes
        _unitOfWork.Repository<Place>().Update(place);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to response
        var response = _mapper.Map<PlaceResponse>(place);
        return response;
    }
}
