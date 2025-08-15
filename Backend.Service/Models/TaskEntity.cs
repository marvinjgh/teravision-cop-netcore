using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Service.Models;

[Table("Task")]
public class TaskEntity : BaseModel
{
    [Key]
    public long Id { get; set; }
    [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    // TODO ProjectId long FK User
    public override string ToString()
    {
        return $"TaskEntity [Id: {Id}, Name: {Name}, Description: {Description}, CreateAt: {CreatedAt}, UpdateAt: {UpdatedAt}, IsDeleted: {(IsDeleted ? "true" : "false")}]";
    }
}
