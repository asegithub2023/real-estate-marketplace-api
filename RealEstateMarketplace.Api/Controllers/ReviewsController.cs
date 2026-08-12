using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Reviews.Commands;
using RealEstateMarketplace.Application.Reviews.Queries;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/reviews")]
[Tags("Reviews")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public ReviewsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("property/{propertyId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get reviews for a property")]
    [EndpointDescription("Returns all reviews associated with the specified property.")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetByPropertyId(int propertyId, CancellationToken cancellationToken)
    {
        var reviews = await _sender.Send(new GetReviewsByPropertyQuery { PropertyId = propertyId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<ReviewDto>>(reviews));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a review by ID")]
    [EndpointDescription("Returns the review matching the specified identifier.")]
    public async Task<ActionResult<ReviewDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var review = await _sender.Send(new GetReviewByIdQuery { Id = id }, cancellationToken);
        return review is null ? NotFound() : Ok(_mapper.Map<ReviewDto>(review));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Create a review")]
    [EndpointDescription("Adds a new review to the specified property.")]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateReviewCommand
        {
            Rating = request.Rating,
            Comment = request.Comment,
            UserId = request.UserId,
            PropertyId = request.PropertyId
        }, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : result.Error!.Code == "user_or_property_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Update a review")]
    [EndpointDescription("Updates an existing review by ID.")]
    public async Task<ActionResult<ReviewDto>> Update(int id, [FromBody] UpdateReviewDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateReviewCommand
        {
            Id = id,
            Rating = request.Rating,
            Comment = request.Comment
        }, cancellationToken);

        return result.IsSuccess ? Ok(result.Value!) : NotFound();
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

