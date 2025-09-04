namespace Backend.Service.DataTransferObjects;

/// <summary>
/// Represents a data transfer object for an error.
/// </summary>
public class ErrorDTO
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public required string Message { set; get; }
}