namespace Backend.Service.Models;

/// <summary>
/// Represents the base model for all entities, containing common properties.
/// </summary>
public abstract class BaseModel
{
    /// <summary>
    /// Gets or sets the creation date and time of the entity.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    /// <summary>
    /// Gets or sets the last update date and time of the entity.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}