namespace Tripilot.Domain.Entities;

/// <summary>
/// Document uploaded for business profile verification
/// </summary>
public class BusinessVerificationDocument : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Foreign key to BusinessProfile
    /// </summary>
    public Guid BusinessProfileId { get; set; }

    /// <summary>
    /// Type of document (e.g., "Business License", "Tax ID", "Certificate")
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Storage URL or identifier
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// Original filename
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// MIME type
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// Whether admin has reviewed this document
    /// </summary>
    public bool IsReviewed { get; set; }

    /// <summary>
    /// Admin notes on this document
    /// </summary>
    public string? ReviewNotes { get; set; }

    // Navigation
    public BusinessProfile? BusinessProfile { get; set; }

    // Auditing
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
