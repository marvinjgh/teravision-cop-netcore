using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Service.Models;

[Table("Project")]
public class Project : BaseEntity
{
    [Key]
    [Column("ProjectId")]
    public long Id { get; set; }

    [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Project [Id: {Id}, Name: {Name ?? "N/A"}, Description: {Description ?? "N/A"}, CreateAt: {CreatedAt}, UpdateAt: {(UpdatedAt.HasValue ? UpdatedAt.Value.ToString() : "N/A")}]";
    }
}
