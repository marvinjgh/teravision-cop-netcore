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
    // TODO ProjectId long FK User
}
