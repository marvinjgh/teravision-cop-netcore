namespace Backend.Service.Models;

public abstract class BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } // Nullable for optional updates
}