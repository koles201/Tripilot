namespace Tripilot.Application.DTOs.Place;

/// <summary>
/// Response DTO for autocomplete suggestions
/// </summary>
public class AutocompleteSuggestion
{
    public Guid? Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "place", "category", "city", "recent"
    public string? Category { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Rating { get; set; }
    public int? ReviewCount { get; set; }
}

/// <summary>
/// Result container for autocomplete suggestions
/// </summary>
public class AutocompleteResult
{
    public List<AutocompleteSuggestion> Suggestions { get; set; } = new();
    public List<string> RecentSearches { get; set; } = new();
}
