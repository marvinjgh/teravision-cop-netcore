using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Service.Models;

/// <summary>
/// Represents a task entity that can be associated with a project.
/// </summary>
[Table("Task")]
public class TaskEntity : BaseModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the task.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the task is deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the foreign key for the associated project.
    /// </summary>
    [ForeignKey("ProjectId")]
    public long? ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the associated project entity.
    /// </summary>
    public ProjectEntity? Project { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"TaskEntity [Id: {Id}, Name: {Name}, Description: {Description}, CreateAt: {CreatedAt}, UpdateAt: {UpdatedAt}, IsDeleted: {(IsDeleted ? "true" : "false")}, ProjectId: {ProjectId?.ToString() ?? "N/A"}]";
    }
}
