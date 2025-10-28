using FluentValidation;
using Tripilot.Application.DTOs.Route;
using Tripilot.Application.Features.Routes.Commands;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Routes.Validators;

public class CreateRouteCommandValidator : AbstractValidator<CreateRouteCommand>
{
    public CreateRouteCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(3, 200).WithMessage("Name must be between 3 and 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .Length(10, 2000).WithMessage("Description must be between 10 and 2000 characters");

        RuleFor(x => x.Difficulty)
            .NotEmpty().WithMessage("Difficulty is required")
            .Must(BeValidDifficulty).WithMessage("Difficulty must be one of: Easy, Moderate, Challenging, Difficult");

        RuleFor(x => x.Privacy)
            .NotEmpty().WithMessage("Privacy is required")
            .Must(BeValidPrivacy).WithMessage("Privacy must be one of: Public, Private, Unlisted");

        RuleFor(x => x.EstimatedDuration)
            .InclusiveBetween(1, 10000).WithMessage("Estimated duration must be between 1 and 10000 minutes");

        When(x => x.TotalDistance.HasValue, () =>
        {
            RuleFor(x => x.TotalDistance!.Value)
                .InclusiveBetween(0, 10000).WithMessage("Total distance must be between 0 and 10000 km");
        });

        When(x => !string.IsNullOrEmpty(x.ImageUrl), () =>
        {
            RuleFor(x => x.ImageUrl)
                .Must(BeValidUrl).WithMessage("Image URL must be a valid URL");
        });

        RuleFor(x => x.Places)
            .NotEmpty().WithMessage("At least one place is required")
            .Must(HaveUniqueOrders).WithMessage("Place orders must be unique")
            .Must(HaveSequentialOrders).WithMessage("Place orders must start from 1 and be sequential");

        RuleForEach(x => x.Places).ChildRules(place =>
        {
            place.RuleFor(p => p.PlaceId)
                .NotEmpty().WithMessage("Place ID is required");

            place.RuleFor(p => p.Order)
                .GreaterThan(0).WithMessage("Order must be greater than 0");

            place.When(p => p.EstimatedTimeAtPlace.HasValue, () =>
            {
                place.RuleFor(p => p.EstimatedTimeAtPlace!.Value)
                    .InclusiveBetween(1, 1440).WithMessage("Estimated time at place must be between 1 and 1440 minutes (24 hours)");
            });

            place.When(p => !string.IsNullOrEmpty(p.Notes), () =>
            {
                place.RuleFor(p => p.Notes)
                    .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");
            });
        });
    }

    private bool BeValidDifficulty(string difficulty)
    {
        return Enum.TryParse<RouteDifficulty>(difficulty, true, out _);
    }

    private bool BeValidPrivacy(string privacy)
    {
        return Enum.TryParse<RoutePrivacy>(privacy, true, out _);
    }

    private bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    private bool HaveUniqueOrders(List<RoutePlaceRequest> places)
    {
        if (places == null || !places.Any()) return true;
        var orders = places.Select(p => p.Order).ToList();
        return orders.Count == orders.Distinct().Count();
    }

    private bool HaveSequentialOrders(List<RoutePlaceRequest> places)
    {
        if (places == null || !places.Any()) return true;
        var orders = places.Select(p => p.Order).OrderBy(o => o).ToList();
        return orders.First() == 1 && orders.SequenceEqual(Enumerable.Range(1, orders.Count));
    }
}
