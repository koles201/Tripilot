namespace Tripilot.Domain.ValueObjects;

/// <summary>
/// Value object representing location coordinates
/// </summary>
public class Location
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }

    private Location() { }

    public Location(double latitude, double longitude, string? address = null, string? city = null, string? country = null, string? postalCode = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
        
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
        Address = address;
        City = city;
        Country = country;
        PostalCode = postalCode;
    }

    public static Location Create(double latitude, double longitude, string? address = null, string? city = null, string? country = null, string? postalCode = null)
    {
        return new Location(latitude, longitude, address, city, country, postalCode);
    }

    public double DistanceTo(Location other)
    {
        // Haversine formula to calculate distance in kilometers
        var R = 6371; // Earth's radius in kilometers
        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
