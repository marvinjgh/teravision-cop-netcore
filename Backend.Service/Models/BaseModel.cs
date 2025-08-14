namespace Backend.Service.Models;

public abstract class BaseModel
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}