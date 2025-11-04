using MediatR;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.Collection;

namespace Tripilot.Application.Features.Collections.Queries;

/// <summary>
/// Query to get user's collections
/// </summary>
public class GetUserCollectionsQuery : IRequest<PaginatedResult<CollectionResponse>>
{
    public Guid UserId { get; set; }
    public Guid? CurrentUserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
