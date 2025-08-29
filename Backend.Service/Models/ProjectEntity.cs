using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Service.Models;

/// <summary>
/// Represents a project entity with related tasks.
/// </summary>
[Table("Project")]
public class ProjectEntity : BaseModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the project.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the project.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the project is deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the collection of tasks associated with the project.
    /// </summary>
    public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();

    /// <inheritdoc/>
    public override string ToString()
    {
        var taskIds = Tasks.Any()
            ? string.Join(", ", Tasks.Select(t => t.Id))
            : "Empty";
        return $"Project [Id: {Id}, Name: {Name}, Description: {Description}, CreateAt: {CreatedAt}, UpdateAt: {UpdatedAt}, IsDeleted: {(IsDeleted ? "true" : "false")}, , TaskIds: [{taskIds}]]";
    }
}
