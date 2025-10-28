using MediatR;

namespace Tripilot.Application.Features.Reviews.Commands;

public class DeleteReviewCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid ReviewerId { get; set; }
}
