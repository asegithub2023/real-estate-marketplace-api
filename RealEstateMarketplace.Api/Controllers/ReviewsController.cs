using AutoMapper;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Api.Utilities;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Reviews.Commands;
using RealEstateMarketplace.Application.Reviews.Queries;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Reviews")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
[ApiVersion("1.0")]
public class ReviewsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly IHateoasHelper _hateoasHelper;

    public ReviewsController(ISender sender, IMapper mapper, IHateoasHelper hateoasHelper)
    {
        _sender = sender;
        _mapper = mapper;
        _hateoasHelper = hateoasHelper;
    }

    [HttpGet("property/{propertyId:int}")]
    [ProducesResponseType(typeof(List<HateoasResponse<ReviewDto>>), StatusCodes.Status200OK)]
    [EndpointSummary("Get reviews for a property")]
    [EndpointDescription("Returns all reviews associated with the specified property with HATEOAS links.")]
    public async Task<ActionResult<List<HateoasResponse<ReviewDto>>>> GetPropertyReviews(int propertyId, CancellationToken cancellationToken)
    {
        var reviews = await _sender.Send(new GetReviewsByPropertyQuery { PropertyId = propertyId }, cancellationToken);
        var reviewDtos = _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
        
        var response = reviewDtos.Select(r => new HateoasResponse<ReviewDto>
        {
            Data = r,
            Links = _hateoasHelper.GenerateReviewResourceLinks(r.Id, propertyId)
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HateoasResponse<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a review by ID")]
    [EndpointDescription("Returns the review matching the specified identifier with HATEOAS links.")]
    public async Task<ActionResult<HateoasResponse<ReviewDto>>> GetReviewById(int id, CancellationToken cancellationToken)
    {
        var review = await _sender.Send(new GetReviewByIdQuery { Id = id }, cancellationToken);
        if (review is null)
            return NotFound();

        var reviewDto = _mapper.Map<ReviewDto>(review);
        var response = new HateoasResponse<ReviewDto>
        {
            Data = reviewDto,
            Links = _hateoasHelper.GenerateReviewResourceLinks(id, review.PropertyId)
        };

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HateoasResponse<ReviewDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Create a review")]
    [EndpointDescription("Adds a new review to the specified property with HATEOAS links.")]
    public async Task<ActionResult<HateoasResponse<ReviewDto>>> CreateReview([FromBody] CreateReviewDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateReviewCommand
        {
            Rating = request.Rating,
            Comment = request.Comment,
            UserId = request.UserId,
            PropertyId = request.PropertyId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error!.Code == "user_or_property_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
        }

        var reviewDto = _mapper.Map<ReviewDto>(result.Value);
        var response = new HateoasResponse<ReviewDto>
        {
            Data = reviewDto,
            Links = _hateoasHelper.GenerateReviewResourceLinks(result.Value!.Id, request.PropertyId)
        };

        return CreatedAtAction(nameof(GetReviewById), new { id = result.Value!.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(HateoasResponse<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Update a review")]
    [EndpointDescription("Updates an existing review by ID with HATEOAS links.")]
    public async Task<ActionResult<HateoasResponse<ReviewDto>>> UpdateReview(int id, [FromBody] UpdateReviewDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateReviewCommand
        {
            Id = id,
            Rating = request.Rating,
            Comment = request.Comment
        }, cancellationToken);

        if (!result.IsSuccess)
            return NotFound();

        var reviewDto = _mapper.Map<ReviewDto>(result.Value);
        var response = new HateoasResponse<ReviewDto>
        {
            Data = reviewDto,
            Links = _hateoasHelper.GenerateReviewResourceLinks(id, result.Value!.PropertyId)
        };

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Delete a review")]
    [EndpointDescription("Deletes the review with the given ID.")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteReviewCommand { Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound();
    }
}

