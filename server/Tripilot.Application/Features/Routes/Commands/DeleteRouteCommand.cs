using MediatR;

namespace Tripilot.Application.Features.Routes.Commands;

public class DeleteRouteCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
