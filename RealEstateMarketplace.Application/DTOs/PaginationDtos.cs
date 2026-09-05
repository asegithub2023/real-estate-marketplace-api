namespace RealEstateMarketplace.Application.DTOs;

using RealEstateMarketplace.Domain.Enums;

public class PagedRequest
{
    private int _page = 1;
    private int _pageSize = 12;
    private const int MaxPageSize = 24;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 12 : (value > MaxPageSize ? MaxPageSize : value);
    }

    public string? Search { get; set; }
    public string OrderBy { get; set; } = "id";
    public bool Descending { get; set; } = true;

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MinBathrooms { get; set; }
    public string? City { get; set; }

    public PropertyType? PropertyType { get; set; }
    public ListingType? ListingType { get; set; }

    public string? SortBy { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}
