using FluentValidation;
using Tripilot.Application.Features.Places.Commands;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Places.Validators;

/// <summary>
/// Validator for CreatePlaceCommand
/// </summary>
public class CreatePlaceCommandValidator : AbstractValidator<CreatePlaceCommand>
{
    public CreatePlaceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(BeValidCategory).WithMessage("Invalid category");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");

        RuleFor(x => x.PostalCode)
            .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters");

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("Phone cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email address")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

        RuleFor(x => x.Website)
            .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.Website))
            .WithMessage("Invalid website URL")
            .MaximumLength(500).WithMessage("Website cannot exceed 500 characters");

        RuleFor(x => x.PriceLevel)
            .InclusiveBetween(1, 4).When(x => x.PriceLevel.HasValue)
            .WithMessage("Price level must be between 1 and 4");

        RuleFor(x => x.Amenities)
            .MaximumLength(1000).WithMessage("Amenities cannot exceed 1000 characters");

        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("Invalid image URL")
            .MaximumLength(1000).WithMessage("Image URL cannot exceed 1000 characters");

        RuleFor(x => x.GalleryImages)
            .MaximumLength(4000).WithMessage("Gallery images cannot exceed 4000 characters");

        RuleFor(x => x.DaysOfWeek)
            .MaximumLength(100).WithMessage("Days of week cannot exceed 100 characters");

        RuleFor(x => x.SpecialNotes)
            .MaximumLength(500).WithMessage("Special notes cannot exceed 500 characters");
    }

    private bool BeValidCategory(string category)
    {
        return Enum.TryParse<PlaceCategory>(category, true, out _);
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
