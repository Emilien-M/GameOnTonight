namespace GameOnTonight.Application.Common;

/// <summary>
/// ViewModel for paginated API responses.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed class PaginatedResultViewModel<T>
{
    /// <summary>
    /// The items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    
    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int Page { get; init; }
    
    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; init; }
    
    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public int TotalCount { get; init; }
    
    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages { get; init; }
    
    /// <summary>
    /// Whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage { get; init; }
    
    /// <summary>
    /// Whether there is a next page.
    /// </summary>
    public bool HasNextPage { get; init; }

    public static PaginatedResultViewModel<T> FromPaginatedResult(PaginatedResult<T> result)
    {
        return new PaginatedResultViewModel<T>
        {
            Items = result.Items,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };
    }
}
