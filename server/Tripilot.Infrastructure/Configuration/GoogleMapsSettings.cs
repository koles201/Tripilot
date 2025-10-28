namespace Tripilot.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for Google Maps API
/// </summary>
public class GoogleMapsSettings
{
    public const string SectionName = "GoogleMaps";

    /// <summary>
    /// Google Maps API Key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Enable or disable Google Maps integration
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Default language for API responses (e.g., "en", "es", "fr")
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";

    /// <summary>
    /// Default region bias for geocoding (e.g., "us", "uk", "pl")
    /// </summary>
    public string DefaultRegion { get; set; } = "us";

    /// <summary>
    /// Maximum number of photo references to return
    /// </summary>
    public int MaxPhotoReferences { get; set; } = 10;

    /// <summary>
    /// Cache duration for geocoding results in minutes
    /// </summary>
    public int GeocodingCacheDurationMinutes { get; set; } = 60;
}
