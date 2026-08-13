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
    List<LinkDto> GeneratePropertyListLinks(int currentPage, int totalPages, int pageSize, string? search = null, string? orderBy = null, bool descending = false);

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

    public List<LinkDto> GeneratePropertyListLinks(int currentPage, int totalPages, int pageSize, string? search = null, string? orderBy = null, bool descending = false)
    {
        var links = new List<LinkDto>();
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return links;

        // Self link
        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetProperties",
            controller: "Properties",
            values: new { page = currentPage, pageSize, search, orderBy, descending }
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

        // First page link
        var firstLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetProperties",
            controller: "Properties",
            values: new { page = 1, pageSize, search, orderBy, descending }
        );

        if (firstLink != null)
        {
            links.Add(new LinkDto
            {
                Href = firstLink,
                Rel = "first",
                Method = "GET"
            });
        }

        // Last page link
        if (totalPages > 0)
        {
            var lastLink = _linkGenerator.GetPathByAction(
                httpContext,
                action: "GetProperties",
                controller: "Properties",
                values: new { page = totalPages, pageSize, search, orderBy, descending }
            );

            if (lastLink != null)
            {
                links.Add(new LinkDto
                {
                    Href = lastLink,
                    Rel = "last",
                    Method = "GET"
                });
            }
        }

        // Previous page link
        if (currentPage > 1)
        {
            var prevLink = _linkGenerator.GetPathByAction(
                httpContext,
                action: "GetProperties",
                controller: "Properties",
                values: new { page = currentPage - 1, pageSize, search, orderBy, descending }
            );

            if (prevLink != null)
            {
                links.Add(new LinkDto
                {
                    Href = prevLink,
                    Rel = "previous",
                    Method = "GET"
                });
            }
        }

        // Next page link
        if (currentPage < totalPages)
        {
            var nextLink = _linkGenerator.GetPathByAction(
                httpContext,
                action: "GetProperties",
                controller: "Properties",
                values: new { page = currentPage + 1, pageSize, search, orderBy, descending }
            );

            if (nextLink != null)
            {
                links.Add(new LinkDto
                {
                    Href = nextLink,
                    Rel = "next",
                    Method = "GET"
                });
            }
        }

        // Create property link (POST)
        var createLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "CreateProperty",
            controller: "Properties"
        );

        if (createLink != null)
        {
            links.Add(new LinkDto
            {
                Href = createLink,
                Rel = "create-property",
                Method = "POST"
            });
        }

        return links.Where(l => l.Href != null).ToList();
    }

    public List<LinkDto> GeneratePropertyResourceLinks(int propertyId)
    {
        var links = new List<LinkDto>();
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
            return links;

        // Self link
        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyById",
            controller: "Properties",
            values: new { id = propertyId }
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

        // Update link
        var updateLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "UpdateProperty",
            controller: "Properties",
            values: new { id = propertyId }
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

        // Delete link
        var deleteLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "DeleteProperty",
            controller: "Properties",
            values: new { id = propertyId }
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

        // Reviews link
        var reviewsLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyReviews",
            controller: "Reviews",
            values: new { propertyId }
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

        // Add to favorites link
        var favoritesLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "AddFavorite",
            controller: "Favorites",
            values: new { propertyId }
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

        // Back to list link
        var listLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetProperties",
            controller: "Properties"
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

        // Self link - get user favorites
        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetUserFavorites",
            controller: "Favorites",
            values: new { userId }
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

        // Remove favorite link
        var removeLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "RemoveFavorite",
            controller: "Favorites",
            values: new { propertyId }
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

        // Get property link
        var propertyLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyById",
            controller: "Properties",
            values: new { id = propertyId }
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

        // Self link
        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetReviewById",
            controller: "Reviews",
            values: new { id = reviewId }
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

        // Update link
        var updateLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "UpdateReview",
            controller: "Reviews",
            values: new { id = reviewId }
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

        // Delete link
        var deleteLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "DeleteReview",
            controller: "Reviews",
            values: new { id = reviewId }
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

        // Property link
        var propertyLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyById",
            controller: "Properties",
            values: new { id = propertyId }
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

        // Property reviews link
        var propertyReviewsLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetPropertyReviews",
            controller: "Reviews",
            values: new { propertyId }
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

        // Self link
        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetConversationById",
            controller: "Conversations",
            values: new { id = conversationId }
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

        // Get messages link
        var messagesLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetConversationMessages",
            controller: "Messages",
            values: new { conversationId }
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

        // Send message link
        var sendMessageLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "SendMessage",
            controller: "Messages",
            values: new { conversationId }
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

        // Update conversation link
        var updateLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "UpdateConversation",
            controller: "Conversations",
            values: new { id = conversationId }
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

        // Self link
        var selfLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetMessageById",
            controller: "Messages",
            values: new { id = messageId }
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

        // Update link
        var updateLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "UpdateMessage",
            controller: "Messages",
            values: new { id = messageId }
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

        // Delete link
        var deleteLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "DeleteMessage",
            controller: "Messages",
            values: new { id = messageId }
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

        // Conversation link
        var conversationLink = _linkGenerator.GetPathByAction(
            httpContext,
            action: "GetConversationById",
            controller: "Conversations",
            values: new { id = conversationId }
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
