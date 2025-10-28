using FluentValidation;
using Tripilot.Application.Features.Reviews.Commands;

namespace Tripilot.Application.Features.Reviews.Validators;

public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
{
    public AddReviewCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .Length(10, 2000).WithMessage("Content must be between 10 and 2000 characters");

        RuleFor(x => x.OverallRating)
            .InclusiveBetween(1, 5).WithMessage("Overall rating must be between 1 and 5");

        When(x => x.CleanlinessRating.HasValue, () =>
        {
            RuleFor(x => x.CleanlinessRating!.Value)
                .InclusiveBetween(1, 5).WithMessage("Cleanliness rating must be between 1 and 5");
        });

        When(x => x.ServiceRating.HasValue, () =>
        {
            RuleFor(x => x.ServiceRating!.Value)
                .InclusiveBetween(1, 5).WithMessage("Service rating must be between 1 and 5");
        });

        When(x => x.ValueRating.HasValue, () =>
        {
            RuleFor(x => x.ValueRating!.Value)
                .InclusiveBetween(1, 5).WithMessage("Value rating must be between 1 and 5");
        });

        When(x => x.LocationRating.HasValue, () =>
        {
            RuleFor(x => x.LocationRating!.Value)
                .InclusiveBetween(1, 5).WithMessage("Location rating must be between 1 and 5");
        });

        RuleFor(x => x)
            .Must(x => (x.PlaceId.HasValue && !x.RouteId.HasValue) || (!x.PlaceId.HasValue && x.RouteId.HasValue))
            .WithMessage("Must provide either PlaceId or RouteId, but not both");
    }
}
