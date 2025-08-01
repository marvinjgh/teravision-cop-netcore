namespace Backend.Service.Models;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } // Nullable for optional updates
}