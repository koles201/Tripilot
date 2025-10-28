using MediatR;
using Tripilot.Application.DTOs.Place;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Query to get a place by ID
/// </summary>
public class GetPlaceByIdQuery : IRequest<PlaceResponse?>
{
    public Guid PlaceId { get; set; }
}
