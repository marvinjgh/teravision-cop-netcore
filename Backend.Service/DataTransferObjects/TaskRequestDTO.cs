using System.ComponentModel.DataAnnotations;

namespace Backend.Service.DataTransferObjects;

/// <summary>
/// Data transfer object for creating or updating a task.
/// </summary>
public class TaskRequestDTO
{
    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the project this task belongs to.
    /// </summary>
    [Required(ErrorMessage = "ProjectId is required")]
    public long ProjectId { get; set; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"TaskRequestDTO [Name: {Name}, Description: {Description}, ProjectId: {ProjectId}]";
    }
}
