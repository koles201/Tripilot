using MediatR;
using Tripilot.Application.DTOs.Collection;

namespace Tripilot.Application.Features.Collections.Queries;

/// <summary>
/// Query to get collection details with routes
/// </summary>
public class GetCollectionByIdQuery : IRequest<CollectionDetailResponse>
{
    public Guid CollectionId { get; set; }
    public Guid? CurrentUserId { get; set; }
}
