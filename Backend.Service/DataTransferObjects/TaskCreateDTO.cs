using System.ComponentModel.DataAnnotations;

namespace Backend.Service.DataTransferObjects;

public class TaskCreateDTO
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(64, ErrorMessage = "Name cannot be longer than 64 characters")]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public long ProjectId { get; set; }

    public override string ToString()
    {
        return $"Task [Name: {Name ?? "N/A"}, Description: {Description ?? "N/A"}, ProjectId: {ProjectId}]";
    }
}
