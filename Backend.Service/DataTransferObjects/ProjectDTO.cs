namespace Backend.Service.DataTransferObjects;

/// <summary>
/// Data Transfer Object (DTO) representing a Project entity.
/// </summary>
public class ProjectDTO
{
    /// <summary>
    /// Gets or sets the unique identifier of the project.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the project.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date and time of the project.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last updated date and time of the project.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the project is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}