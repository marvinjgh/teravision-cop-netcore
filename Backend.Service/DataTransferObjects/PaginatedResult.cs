namespace Backend.Service.DataTransferObjects;

/// <summary>
/// Represents a paginated result set with metadata for pagination.
/// </summary>
public class PaginatedResult<T>
{
    /// <summary>
    /// Gets or sets the collection of items for the current page.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Gets or sets the total number of items across all pages.
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    public long CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public long PageSize { get; set; }

    /// <summary>
    /// Gets the total number of pages based on <see cref="TotalCount"/> and <see cref="PageSize"/>.
    /// </summary>
    public long TotalPages => (long)Math.Ceiling((double)TotalCount / PageSize);
}
