using MediatR;
using Tripilot.Application.DTOs.Collection;

namespace Tripilot.Application.Features.Collections.Commands;

/// <summary>
/// Command to create a new route collection
/// </summary>
public class CreateCollectionCommand : IRequest<CollectionResponse>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? CoverImageUrl { get; set; }
}
