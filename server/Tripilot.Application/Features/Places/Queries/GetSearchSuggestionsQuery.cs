using MediatR;
using Tripilot.Application.DTOs.Place;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Query for getting autocomplete suggestions
/// </summary>
public class GetSearchSuggestionsQuery : IRequest<AutocompleteResult>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int MaxSuggestions { get; set; } = 10;
}
