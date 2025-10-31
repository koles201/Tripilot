using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Place;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Handler for generating autocomplete suggestions
/// </summary>
public class GetSearchSuggestionsQueryHandler : IRequestHandler<GetSearchSuggestionsQuery, AutocompleteResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSearchSuggestionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AutocompleteResult> Handle(GetSearchSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var suggestions = new List<AutocompleteSuggestion>();

        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return new AutocompleteResult { Suggestions = suggestions };
        }

        var searchTerm = request.SearchTerm.ToLower().Trim();

        // 1. Search for matching places (name starts with or contains search term)
        var placeSuggestions = await _unitOfWork.Repository<Domain.Entities.Place>().GetQueryable()
            .Where(p => p.IsActive &&
                       (EF.Functions.Like(p.Name.ToLower(), $"{searchTerm}%") ||
                        EF.Functions.Like(p.Name.ToLower(), $"% {searchTerm}%")))
            .OrderByDescending(p => p.ViewCount)
            .ThenByDescending(p => p.AverageRating)
            .Take(5)
            .Select(p => new AutocompleteSuggestion
            {
                Id = p.Id,
                Text = p.Name,
                Type = "place",
                Category = p.Category.ToString(),
                Location = p.Location.City != null && p.Location.Country != null ? $"{p.Location.City}, {p.Location.Country}" : null,
                ImageUrl = p.ImageUrl,
                Rating = p.AverageRating,
                ReviewCount = p.ReviewCount
            })
            .ToListAsync(cancellationToken);

        suggestions.AddRange(placeSuggestions);

        // 2. Search for matching categories
        var categorySuggestions = await _unitOfWork.Repository<Domain.Entities.Place>().GetQueryable()
            .Where(p => p.IsActive && EF.Functions.Like(p.Category.ToString().ToLower(), $"%{searchTerm}%"))
            .GroupBy(p => p.Category)
            .Select(g => new AutocompleteSuggestion
            {
                Text = g.Key.ToString(),
                Type = "category",
                Category = g.Key.ToString()
            })
            .Take(3)
            .ToListAsync(cancellationToken);

        suggestions.AddRange(categorySuggestions);

        // 3. Search for matching cities
        var citySuggestions = await _unitOfWork.Repository<Domain.Entities.Place>().GetQueryable()
            .Where(p => p.IsActive && p.Location.City != null &&
                       EF.Functions.Like(p.Location.City.ToLower(), $"%{searchTerm}%"))
            .GroupBy(p => new { p.Location.City, p.Location.Country })
            .Select(g => new AutocompleteSuggestion
            {
                Text = $"{g.Key.City}, {g.Key.Country}",
                Type = "city",
                Location = $"{g.Key.City}, {g.Key.Country}"
            })
            .Take(2)
            .ToListAsync(cancellationToken);

        suggestions.AddRange(citySuggestions);

        // Limit total suggestions
        return new AutocompleteResult
        {
            Suggestions = suggestions.Take(request.MaxSuggestions).ToList()
        };
    }
}
