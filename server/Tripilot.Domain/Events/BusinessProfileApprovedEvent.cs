using MediatR;

namespace Tripilot.Domain.Events;

public record BusinessProfileApprovedEvent(Guid BusinessProfileId, Guid UserId, DateTime ApprovedAt) : INotification;
