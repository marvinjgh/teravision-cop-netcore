using System.ComponentModel.DataAnnotations;

namespace Backend.Service.Models;

public class Project
{
    public long Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [StringLength(64, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public override string ToString()
    {
        return $"Project [Id: {Id}, Name: {Name ?? "N/A"}, Description: {Description ?? "N/A"}, CreateAt: {CreateAt}, UpdateAt: {(UpdateAt.HasValue ? UpdateAt.Value.ToString() : "N/A")}]";
    }
}
