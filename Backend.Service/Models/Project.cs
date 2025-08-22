using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Service.Models;

[Table("Project")]
public class Project : BaseModel
{
    [Key]
    public long Id { get; set; }
    [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    public override string ToString()
    {
        var taskIds = Tasks.Any()
            ? string.Join(", ", Tasks.Select(t => t.Id))
            : "Empty";
        return $"Project [Id: {Id}, Name: {Name}, Description: {Description}, CreateAt: {CreatedAt}, UpdateAt: {UpdatedAt}, IsDeleted: {(IsDeleted ? "true" : "false")}, , TaskIds: [{taskIds}]]";
    }
}
