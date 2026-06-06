namespace ECommerce.Application.ResourceParameters;

/// <summary>
/// Shared pagination and search parameters for all list queries.
/// </summary>
public abstract class BaseResourceParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>Free-text search across name and description.</summary>
    public string? SearchQuery { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate   { get; set; }
}
