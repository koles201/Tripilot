using MediatR;

namespace Tripilot.Domain.Events;

public record BusinessProfileSubmittedEvent(Guid BusinessProfileId, Guid UserId, DateTime SubmittedAt) : INotification;
