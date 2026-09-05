namespace RealEstateMarketplace.Application.DTOs;

public class LinkDto
{

    public string? Href { get; set; }

    public string? Rel { get; set; }

    public string Method { get; set; } = "GET";
}

public class HateoasPagedResponse<T>
{

    public List<T> Data { get; set; } = new();

    public PageMetadata Meta { get; set; } = new();

    public List<LinkDto> Links { get; set; } = new();
}

public class PageMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}

public class HateoasResponse<T>
{

    public T? Data { get; set; }

    public List<LinkDto> Links { get; set; } = new();
}
