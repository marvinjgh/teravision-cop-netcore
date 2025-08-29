namespace Backend.Service.DataTransferObjects;

/// <summary>
/// Data Transfer Object for TaskEntity.
/// </summary>
public class TaskDTO
{
    /// <summary>
    /// Gets or sets the unique identifier for the task.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
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
    /// Gets or sets a value indicating whether the task is deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the project identifier associated with the task.
    /// </summary>
    public long? ProjectId { get; set; }
}

