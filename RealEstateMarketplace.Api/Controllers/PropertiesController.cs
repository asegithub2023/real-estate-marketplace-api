using AutoMapper;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Api.Utilities;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Application.Properties.Commands;
using RealEstateMarketplace.Application.Properties.Queries;
using Scalar.AspNetCore;
using RealEstateMarketplace.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Properties")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
[ApiVersion("1.0")]
public class PropertiesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly ICachedPropertyService _cachedPropertyService;
    private readonly IPropertyService _propertyService;
    private readonly IPropertyImageService _propertyImageService;
    private readonly IHateoasHelper _hateoasHelper;
    private readonly ICloudinaryService _cloudinaryService;

    public PropertiesController(
        ISender sender,
        IMapper mapper,
        ICachedPropertyService cachedPropertyService,
        IPropertyService propertyService,
        IPropertyImageService propertyImageService,
        IHateoasHelper hateoasHelper,
        ICloudinaryService cloudinaryService)
    {
        _sender = sender;
        _mapper = mapper;
        _cachedPropertyService = cachedPropertyService;
        _propertyService = propertyService;
        _propertyImageService = propertyImageService;
        _hateoasHelper = hateoasHelper;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<PropertyDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get all properties")]
    [EndpointDescription("Returns all available properties.")]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetAll(CancellationToken cancellationToken)
    {
        var properties = await _cachedPropertyService.GetAllPropertiesAsync(cancellationToken);
        return Ok(properties);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HateoasPagedResponse<PropertyDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Search and filter properties")]
    [EndpointDescription("Returns paginated properties with search, filtering, and sorting capabilities. Includes HATEOAS links for navigation.")]
    public async Task<ActionResult<HateoasPagedResponse<PropertyDto>>> GetProperties([FromQuery] PagedRequest request, CancellationToken cancellationToken)
    {
        var result = await _propertyService.GetPropertiesAsync(request, cancellationToken);

        var response = new HateoasPagedResponse<PropertyDto>
        {
            Data = result.Items,
            Meta = new PageMetadata
            {
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                HasNext = result.HasNext,
                HasPrevious = result.HasPrevious
            },
            Links = _hateoasHelper.GeneratePropertyListLinks(
    result.Page,
    result.TotalPages,
    request)
        };

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HateoasResponse<PropertyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a property by ID")]
    [EndpointDescription("Returns the property matching the specified identifier with HATEOAS links.")]
    public async Task<ActionResult<HateoasResponse<PropertyDto>>> GetPropertyById(int id, CancellationToken cancellationToken)
    {
        var property = await _cachedPropertyService.GetPropertyAsync(id, cancellationToken);
        if (property is null)
            return NotFound();

        var response = new HateoasResponse<PropertyDto>
        {
            Data = property,
            Links = _hateoasHelper.GeneratePropertyResourceLinks(id)
        };

        return Ok(response);
    }

    [HttpGet("owner/{ownerId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<PropertyDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get properties by owner")]
    [EndpointDescription("Returns all properties owned by the specified user.")]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetByOwnerId(int ownerId, CancellationToken cancellationToken)
    {
        var properties = await _sender.Send(new GetPropertiesByOwnerIdQuery { OwnerId = ownerId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<PropertyDto>>(properties));
    }

    [HttpPost]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Create a property")]
    [EndpointDescription("Creates a new property record with optional images.")]
    public async Task<ActionResult<PropertyDto>> Create(
        [FromForm] CreatePropertyDto request,
        CancellationToken cancellationToken)
    {
        if (request.Images is null || request.Images.Count == 0)
        {
            return BadRequest("At least one property image is required.");
        }

        if (request.Images.Count > 7)
        {
            return BadRequest("You can upload a maximum of 7 images.");
        }

        var imageUrls = new List<string>();

        try
        {
            foreach (var image in request.Images)
{
    if (image is null || image.Length == 0)
        return BadRequest("Image file cannot be empty.");

    var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };

    if (!allowedTypes.Contains(image.ContentType.ToLowerInvariant()))
        return BadRequest("Only JPEG, PNG, and WebP images are allowed.");

    var uploadResult = await _cloudinaryService.UploadImageAsync(image);
    imageUrls.Add(uploadResult.ImageUrl);
}
        }
        catch (Exception ex)
        {
            return BadRequest($"Image upload failed: {ex.Message}");
        }

        if (imageUrls.Count == 0)
        {
            return BadRequest("At least one valid property image is required.");
        }

        var commandResult = await _sender.Send(new CreatePropertyCommand
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            City = request.City,
            Address = request.Address,
            Country = request.Country,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            Rooms = request.Rooms,
            Area = request.Area,
            PropertyType = request.PropertyType,
            ListingType = request.ListingType,
            OwnerId = request.OwnerId,
            ImageUrls = imageUrls
        }, cancellationToken);

        if (!commandResult.IsSuccess)
        {
            return commandResult.Error!.Code == "owner_not_found"
                ? NotFound(commandResult.Error.Message)
                : BadRequest(commandResult.Error.Message);
        }

        await _cachedPropertyService.InvalidatePropertyCacheAsync(
            null,
            cancellationToken);

      
var propertyDto = _mapper.Map<PropertyDto>(commandResult.Value);

return CreatedAtAction(
    nameof(GetPropertyById),
    new { id = commandResult.Value.Id },
    propertyDto);

    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Update a property")]
    [EndpointDescription("Updates an existing property by ID. Only the property's owner or an admin may update it.")]
    public async Task<ActionResult<PropertyDto>> Update(int id, [FromBody] UpdatePropertyDto request, CancellationToken cancellationToken)
    {
        var existing = await _cachedPropertyService.GetPropertyAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isOwner = int.TryParse(currentUserId, out var userId) && existing.OwnerId == userId;
        var isAdmin = User.IsInRole("Admin");

        if (!isOwner && !isAdmin)
        {
            return Forbid();
        }

        var result = await _sender.Send(new UpdatePropertyCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            City = request.City,
            Address = request.Address,
            Country = request.Country,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            Rooms = request.Rooms,
            Area = request.Area,
            Status = request.Status
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound();
        }

        await _cachedPropertyService.InvalidatePropertyCacheAsync(id, cancellationToken);
        return Ok(result.Value!);
    }

    [HttpPost("{id:int}/images")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Add images to a property")]
    [EndpointDescription("Uploads and adds new images to an existing property. Only the property's owner or an admin may do this. Max 7 images total per property.")]
    public async Task<ActionResult<PropertyDto>> AddImages(int id, List<IFormFile> images, CancellationToken cancellationToken)
    {
        var existing = await _cachedPropertyService.GetPropertyAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (!IsOwnerOrAdmin(existing.OwnerId))
        {
            return Forbid();
        }

        if (images is null || images.Count == 0)
        {
            return BadRequest("At least one image is required.");
        }

        var currentImageCount = existing.Images.Count;
        if (currentImageCount + images.Count > 7)
        {
            return BadRequest($"A property can have a maximum of 7 images. This property already has {currentImageCount}.");
        }

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };

        foreach (var image in images)
        {
            if (image is null || image.Length == 0)
            {
                return BadRequest("Image file cannot be empty.");
            }

            if (!allowedTypes.Contains(image.ContentType.ToLowerInvariant()))
            {
                return BadRequest("Only JPEG, PNG, and WebP images are allowed.");
            }
        }

        foreach (var image in images)
        {
            var uploadResult = await _cloudinaryService.UploadImageAsync(image);
            await _propertyImageService.CreateAsync(new CreatePropertyImageDto
            {
                ImageUrl = uploadResult.ImageUrl,
                PropertyId = id
            }, cancellationToken);
        }

        await _cachedPropertyService.InvalidatePropertyCacheAsync(id, cancellationToken);

        var updated = await _cachedPropertyService.GetPropertyAsync(id, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}/images/{imageId:int}")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Remove an image from a property")]
    [EndpointDescription("Deletes one image from an existing property. Only the property's owner or an admin may do this. A property must keep at least one image.")]
    public async Task<ActionResult<PropertyDto>> DeleteImage(int id, int imageId, CancellationToken cancellationToken)
    {
        var existing = await _cachedPropertyService.GetPropertyAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (!IsOwnerOrAdmin(existing.OwnerId))
        {
            return Forbid();
        }

        if (!existing.Images.Any(i => i.Id == imageId))
        {
            return NotFound();
        }

        if (existing.Images.Count <= 1)
        {
            return BadRequest("A property must have at least one image.");
        }

        await _propertyImageService.DeleteAsync(imageId, cancellationToken);
        await _cachedPropertyService.InvalidatePropertyCacheAsync(id, cancellationToken);

        var updated = await _cachedPropertyService.GetPropertyAsync(id, cancellationToken);
        return Ok(updated);
    }

    private bool IsOwnerOrAdmin(int propertyOwnerId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isOwner = int.TryParse(currentUserId, out var userId) && propertyOwnerId == userId;
        var isAdmin = User.IsInRole("Admin");
        return isOwner || isAdmin;
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Delete a property")]
    [EndpointDescription("Deletes the specified property. Only the property's owner or an admin may do this.")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _cachedPropertyService.GetPropertyAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (!IsOwnerOrAdmin(existing.OwnerId))
        {
            return Forbid();
        }

        var result = await _sender.Send(new DeletePropertyCommand { Id = id }, cancellationToken);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        await _cachedPropertyService.InvalidatePropertyCacheAsync(id, cancellationToken);
        return NoContent();
    }
}