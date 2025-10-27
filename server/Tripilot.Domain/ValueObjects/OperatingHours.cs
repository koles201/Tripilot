namespace Tripilot.Domain.ValueObjects;

/// <summary>
/// Value object representing operating hours
/// </summary>
public class OperatingHours
{
    public TimeSpan? OpenTime { get; private set; }
    public TimeSpan? CloseTime { get; private set; }
    public string? DaysOfWeek { get; private set; } // Comma-separated: "Mon,Tue,Wed,Thu,Fri"
    public bool Is24Hours { get; private set; }
    public string? SpecialNotes { get; private set; }

    private OperatingHours() { }

    public OperatingHours(TimeSpan? openTime, TimeSpan? closeTime, string? daysOfWeek, bool is24Hours = false, string? specialNotes = null)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
        DaysOfWeek = daysOfWeek;
        Is24Hours = is24Hours;
        SpecialNotes = specialNotes;
    }

    public static OperatingHours Create(TimeSpan? openTime, TimeSpan? closeTime, string? daysOfWeek, bool is24Hours = false, string? specialNotes = null)
    {
        return new OperatingHours(openTime, closeTime, daysOfWeek, is24Hours, specialNotes);
    }

    public static OperatingHours Create24Hours()
    {
        return new OperatingHours(null, null, "Mon,Tue,Wed,Thu,Fri,Sat,Sun", true);
    }
}
