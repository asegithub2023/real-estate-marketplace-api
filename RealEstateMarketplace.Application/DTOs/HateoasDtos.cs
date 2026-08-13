namespace RealEstateMarketplace.Application.DTOs;

/// <summary>
/// Represents a HATEOAS link for API responses.
/// Follows REST hypermedia conventions to enable client navigation through related resources.
/// </summary>
public class LinkDto
{
    /// <summary>
    /// The URI/path for the link
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// The semantic meaning of the link (e.g., "self", "get-property", "delete", "create-property")
    /// </summary>
    public string? Rel { get; set; }

    /// <summary>
    /// HTTP method to use when following the link (GET, POST, PUT, DELETE, etc.)
    /// </summary>
    public string Method { get; set; } = "GET";
}

/// <summary>
/// Wrapper for paginated responses with HATEOAS links.
/// Enables clients to discover related actions without hard-coding URLs.
/// </summary>
public class HateoasPagedResponse<T>
{
    /// <summary>
    /// The paginated data items
    /// </summary>
    public List<T> Data { get; set; } = new();

    /// <summary>
    /// Metadata about the pagination
    /// </summary>
    public PageMetadata Meta { get; set; } = new();

    /// <summary>
    /// HATEOAS links for navigation and actions
    /// </summary>
    public List<LinkDto> Links { get; set; } = new();
}

/// <summary>
/// Metadata for paginated responses
/// </summary>
public class PageMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}

/// <summary>
/// Wrapper for single resource responses with HATEOAS links.
/// </summary>
public class HateoasResponse<T>
{
    /// <summary>
    /// The resource data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// HATEOAS links for navigation and actions
    /// </summary>
    public List<LinkDto> Links { get; set; } = new();
}
