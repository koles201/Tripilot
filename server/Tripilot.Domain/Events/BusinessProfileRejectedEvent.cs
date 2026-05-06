using MediatR;

namespace Tripilot.Domain.Events;

public record BusinessProfileRejectedEvent(Guid BusinessProfileId, Guid UserId, string? Reason, DateTime RejectedAt) : INotification;
