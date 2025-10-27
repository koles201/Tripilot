namespace Tripilot.Domain.ValueObjects;

/// <summary>
/// Value object representing contact information
/// </summary>
public class ContactInfo
{
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }

    private ContactInfo() { }

    public ContactInfo(string? phone, string? email, string? website)
    {
        Phone = phone;
        Email = email;
        Website = website;
    }

    public static ContactInfo Create(string? phone, string? email, string? website)
    {
        return new ContactInfo(phone, email, website);
    }
}
