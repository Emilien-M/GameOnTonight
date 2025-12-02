namespace GameOnTonight.Application.Common;

/// <summary>
/// Represents a paginated result containing a subset of items and pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed class PaginatedResult<T>
{
    /// <summary>
    /// The items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; }
    
    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int Page { get; }
    
    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; }
    
    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public int TotalCount { get; }
    
    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    
    /// <summary>
    /// Whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
    
    /// <summary>
    /// Whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    public PaginatedResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Creates a paginated result from an IQueryable source.
    /// </summary>
    public static PaginatedResult<T> Create(IEnumerable<T> source, int page, int pageSize, int totalCount)
    {
        var items = source.ToList().AsReadOnly();
        return new PaginatedResult<T>(items, page, pageSize, totalCount);
    }
}
