namespace Tripilot.Domain.Entities;

/// <summary>
/// Interface for entities that track creation and modification timestamps
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Timestamp when the entity was created
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created the entity
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// Timestamp when the entity was last modified
    /// </summary>
    DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// User who last modified the entity
    /// </summary>
    string? ModifiedBy { get; set; }
}
