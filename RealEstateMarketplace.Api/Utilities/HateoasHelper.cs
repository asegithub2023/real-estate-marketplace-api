using Asp.Versioning;
using Microsoft.AspNetCore.Routing;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Api.Utilities;

/// <summary>
/// Helper service for generating HATEOAS links in API responses.
/// Provides consistent link generation patterns across all controllers.
/// </summary>
public interface IHateoasHelper
{
    /// <summary>
    /// Generate links for a list of properties
    /// </summary>
    // after
List<LinkDto> GeneratePropertyListLinks(int currentPage, int totalPages, PagedRequest request);

    /// <summary>
    /// Generate links for a single property resource
    /// </summary>
    List<LinkDto> GeneratePropertyResourceLinks(int propertyId);

    /// <summary>
    /// Generate links for a single favorite resource
    /// </summary>
    List<LinkDto> GenerateFavoriteResourceLinks(int propertyId, int userId);

    /// <summary>
    /// Generate links for a review resource
    /// </summary>
    List<LinkDto> GenerateReviewResourceLinks(int reviewId, int propertyId);

    /// <summary>
    /// Generate links for a conversation resource
    /// </summary>
    List<LinkDto> GenerateConversationResourceLinks(int conversationId);

    /// <summary>
    /// Generate links for a message resource
    /// </summary>
    List<LinkDto> GenerateMessageResourceLinks(int messageId, int conversationId);
}

public class HateoasHelper : IHateoasHelper
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HateoasHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    private static string GetVersionedRouteValue(HttpContext httpContext)
    {
        if (httpContext.Request.RouteValues.TryGetValue("version", out var versionValue) && versionValue is not null)
        {
            return versionValue.ToString() ?? "1.0";
        }

        return "1.0";
    }

    private static object BuildVersionedRouteValues(HttpContext httpContext, object routeValues)
    {
        var values = new RouteValueDictionary(routeValues);

        if (!values.ContainsKey("version"))
        {
            values["version"] = GetVersionedRouteValue(httpContext);
        }

        return values;
    }

   public List<LinkDto> GeneratePropertyListLinks(int currentPage, int totalPages, PagedRequest request)
{
    var links = new List<LinkDto>();
    var httpContext = _httpContextAccessor.HttpContext;

    if (httpContext == null)
        return links;

       object RouteValuesFor(int page) => BuildVersionedRouteValues(httpContext, new
    {
        page,
        pageSize = request.PageSize,
        search = request.Search,
        orderBy = request.OrderBy,
        descending = request.Descending,
        minPrice = request.MinPrice,
        maxPrice = request.MaxPrice,
        minBedrooms = request.MinBedrooms,
        minBathrooms = request.MinBathrooms,
        city = request.City,
        propertyType = request.PropertyType,
        listingType = request.ListingType,
        sortBy = request.SortBy
    });

    var selfLink = _linkGenerator.GetPathByAction(
        httpContext, action: "GetProperties", controller: "Properties", values: RouteValuesFor(currentPage));

    if (selfLink != null)
        links.Add(new LinkDto { Href = selfLink, Rel = "self", Method = "GET" });

    var firstLink = _linkGenerator.GetPathByAction(
        httpContext, action: "GetProperties", controller: "Properties", values: RouteValuesFor(1));

    if (firstLink != null)
        links.Add(new LinkDto { Href = firstLink, Rel = "first", Method = "GET" });

    if (totalPages > 0)
    {
        var lastLink = _linkGenerator.GetPathByAction(
            httpContext, action: "GetProperties", controller: "Properties", values: RouteValuesFor(totalPages));

        if (lastLink != null)
            links.Add(new LinkDto { Href = lastLink, Rel = "last", Method = "GET" });
    }

    if (currentPage > 1)
    {
        var prevLink = _linkGenerator.GetPathByAction(
            httpContext, action: "GetProperties", controller: "Properties", values: RouteValuesFor(currentPage - 1));

        if (prevLink != null)
            links.Add(new LinkDto { Href = prevLink, Rel = "previous", Method = "GET" });
    }

    if (currentPage < totalPages)
    {
        var nextLink = _linkGenerator.GetPathByAction(
            httpContext, action: "GetProperties", controller: "Properties", values: RouteValuesFor(currentPage + 1));

        if (nextLink != null)
            links.Add(new LinkDto { Href = nextLink, Rel = "next", Method = "GET" });
    }

    var createLink = _linkGenerator.GetPathByAction(
        httpContext, action: "CreateProperty", controller: "Properties", values: BuildVersionedRouteValues(httpContext, new { }));

    if (createLink != null)
        links.Add(new LinkDto { Href = createLink, Rel = "create-property", Method = "POST" });

    return links.Where(l => l.Href != null).ToList();
}

    public List<LinkDto> GeneratePropertyResourceLinks(int propertyId)
    {
        var links = new List<LinkDto>();
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return links;

        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyById",
            controller: "Properties",
            values: BuildVersionedRouteValues(httpContext, new { id = propertyId })
        );

        if (selfLink != null)
        {
            links.Add(new LinkDto
            {
                Href = selfLink,
                Rel = "self",
                Method = "GET"
            });
        }

        var updateLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "UpdateProperty",
            controller: "Properties",
            values: BuildVersionedRouteValues(httpContext, new { id = propertyId })
        );

        if (updateLink != null)
        {
            links.Add(new LinkDto
            {
                Href = updateLink,
                Rel = "update",
                Method = "PUT"
            });
        }

        var deleteLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "DeleteProperty",
            controller: "Properties",
            values: BuildVersionedRouteValues(httpContext, new { id = propertyId })
        );

        if (deleteLink != null)
        {
            links.Add(new LinkDto
            {
                Href = deleteLink,
                Rel = "delete",
                Method = "DELETE"
            });
        }

        var reviewsLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyReviews",
            controller: "Reviews",
            values: BuildVersionedRouteValues(httpContext, new { propertyId })
        );

        if (reviewsLink != null)
        {
            links.Add(new LinkDto
            {
                Href = reviewsLink,
                Rel = "get-reviews",
                Method = "GET"
            });
        }

        var favoritesLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "AddFavorite",
            controller: "Favorites",
            values: BuildVersionedRouteValues(httpContext, new { propertyId })
        );

        if (favoritesLink != null)
        {
            links.Add(new LinkDto
            {
                Href = favoritesLink,
                Rel = "add-favorite",
                Method = "POST"
            });
        }

        var listLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetProperties",
            controller: "Properties",
            values: BuildVersionedRouteValues(httpContext, new { })
        );

        if (listLink != null)
        {
            links.Add(new LinkDto
            {
                Href = listLink,
                Rel = "property-list",
                Method = "GET"
            });
        }

        return links.Where(l => l.Href != null).ToList();
    }

    public List<LinkDto> GenerateFavoriteResourceLinks(int propertyId, int userId)
    {
        var links = new List<LinkDto>();
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return links;

        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetUserFavorites",
            controller: "Favorites",
            values: BuildVersionedRouteValues(httpContext, new { userId })
        );

        if (selfLink != null)
        {
            links.Add(new LinkDto
            {
                Href = selfLink,
                Rel = "self",
                Method = "GET"
            });
        }

        var removeLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "RemoveFavorite",
            controller: "Favorites",
            values: BuildVersionedRouteValues(httpContext, new { propertyId })
        );

        if (removeLink != null)
        {
            links.Add(new LinkDto
            {
                Href = removeLink,
                Rel = "remove-favorite",
                Method = "DELETE"
            });
        }

        var propertyLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyById",
            controller: "Properties",
            values: BuildVersionedRouteValues(httpContext, new { id = propertyId })
        );

        if (propertyLink != null)
        {
            links.Add(new LinkDto
            {
                Href = propertyLink,
                Rel = "get-property",
                Method = "GET"
            });
        }

        return links.Where(l => l.Href != null).ToList();
    }

    public List<LinkDto> GenerateReviewResourceLinks(int reviewId, int propertyId)
    {
        var links = new List<LinkDto>();
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return links;

        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetReviewById",
            controller: "Reviews",
            values: BuildVersionedRouteValues(httpContext, new { id = reviewId })
        );

        if (selfLink != null)
        {
            links.Add(new LinkDto
            {
                Href = selfLink,
                Rel = "self",
                Method = "GET"
            });
        }

        var updateLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "UpdateReview",
            controller: "Reviews",
            values: BuildVersionedRouteValues(httpContext, new { id = reviewId })
        );

        if (updateLink != null)
        {
            links.Add(new LinkDto
            {
                Href = updateLink,
                Rel = "update",
                Method = "PUT"
            });
        }

        var deleteLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "DeleteReview",
            controller: "Reviews",
            values: BuildVersionedRouteValues(httpContext, new { id = reviewId })
        );

        if (deleteLink != null)
        {
            links.Add(new LinkDto
            {
                Href = deleteLink,
                Rel = "delete",
                Method = "DELETE"
            });
        }

        var propertyLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyById",
            controller: "Properties",
            values: BuildVersionedRouteValues(httpContext, new { id = propertyId })
        );

        if (propertyLink != null)
        {
            links.Add(new LinkDto
            {
                Href = propertyLink,
                Rel = "property",
                Method = "GET"
            });
        }

        var propertyReviewsLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyReviews",
            controller: "Reviews",
            values: BuildVersionedRouteValues(httpContext, new { propertyId })
        );

        if (propertyReviewsLink != null)
        {
            links.Add(new LinkDto
            {
                Href = propertyReviewsLink,
                Rel = "property-reviews",
                Method = "GET"
            });
        }

        return links.Where(l => l.Href != null).ToList();
    }

    public List<LinkDto> GenerateConversationResourceLinks(int conversationId)
    {
        var links = new List<LinkDto>();
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return links;

        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetConversationById",
            controller: "Conversations",
            values: BuildVersionedRouteValues(httpContext, new { id = conversationId })
        );

        if (selfLink != null)
        {
            links.Add(new LinkDto
            {
                Href = selfLink,
                Rel = "self",
                Method = "GET"
            });
        }

        var messagesLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetConversationMessages",
            controller: "Messages",
            values: BuildVersionedRouteValues(httpContext, new { conversationId })
        );

        if (messagesLink != null)
        {
            links.Add(new LinkDto
            {
                Href = messagesLink,
                Rel = "messages",
                Method = "GET"
            });
        }

        var sendMessageLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "SendMessage",
            controller: "Messages",
            values: BuildVersionedRouteValues(httpContext, new { conversationId })
        );

        if (sendMessageLink != null)
        {
            links.Add(new LinkDto
            {
                Href = sendMessageLink,
                Rel = "send-message",
                Method = "POST"
            });
        }

        var updateLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "UpdateConversation",
            controller: "Conversations",
            values: BuildVersionedRouteValues(httpContext, new { id = conversationId })
        );

        if (updateLink != null)
        {
            links.Add(new LinkDto
            {
                Href = updateLink,
                Rel = "update",
                Method = "PUT"
            });
        }

        return links.Where(l => l.Href != null).ToList();
    }

    public List<LinkDto> GenerateMessageResourceLinks(int messageId, int conversationId)
    {
        var links = new List<LinkDto>();
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return links;

        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetMessageById",
            controller: "Messages",
            values: BuildVersionedRouteValues(httpContext, new { id = messageId })
        );

        if (selfLink != null)
        {
            links.Add(new LinkDto
            {
                Href = selfLink,
                Rel = "self",
                Method = "GET"
            });
        }

        var updateLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "UpdateMessage",
            controller: "Messages",
            values: BuildVersionedRouteValues(httpContext, new { id = messageId })
        );

        if (updateLink != null)
        {
            links.Add(new LinkDto
            {
                Href = updateLink,
                Rel = "update",
                Method = "PUT"
            });
        }

        var deleteLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "DeleteMessage",
            controller: "Messages",
            values: BuildVersionedRouteValues(httpContext, new { id = messageId })
        );

        if (deleteLink != null)
        {
            links.Add(new LinkDto
            {
                Href = deleteLink,
                Rel = "delete",
                Method = "DELETE"
            });
        }

        var conversationLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetConversationById",
            controller: "Conversations",
            values: BuildVersionedRouteValues(httpContext, new { id = conversationId })
        );

        if (conversationLink != null)
        {
            links.Add(new LinkDto
            {
                Href = conversationLink,
                Rel = "conversation",
                Method = "GET"
            });
        }

        return links.Where(l => l.Href != null).ToList();
    }
}
