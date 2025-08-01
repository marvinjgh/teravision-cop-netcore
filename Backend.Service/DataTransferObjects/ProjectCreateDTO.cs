using System.ComponentModel.DataAnnotations;

namespace Backend.Service.DataTransferObjects;

public class ProjectCreateDTO
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public override string ToString()
    {
        return $"Project [Name: {Name ?? "N/A"}, Description: {Description ?? "N/A"}]";
    }
}
