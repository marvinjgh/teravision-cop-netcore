using System.ComponentModel.DataAnnotations;

namespace Backend.Service.DataTransferObjects;

/// <summary>
/// Data transfer object for creating or updating a project.
/// </summary>
public class ProjectRequestDTO
{
    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the description of the project.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"ProjectRequestDTO [Name: {Name ?? "N/A"}, Description: {Description ?? "N/A"}]";
    }
}
