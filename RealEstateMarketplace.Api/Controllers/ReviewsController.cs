using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Reviews.Commands;
using RealEstateMarketplace.Application.Reviews.Queries;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly ISender _sender;

    public ReviewsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("property/{propertyId:int}")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetByPropertyId(int propertyId, CancellationToken cancellationToken)
    {
        var reviews = await _sender.Send(new GetReviewsByPropertyQuery { PropertyId = propertyId }, cancellationToken);
        return Ok(reviews);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReviewDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var review = await _sender.Send(new GetReviewByIdQuery { Id = id }, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto request, CancellationToken cancellationToken)
    {
        var review = await _sender.Send(new CreateReviewCommand
        {
            Rating = request.Rating,
            Comment = request.Comment,
            UserId = request.UserId,
            PropertyId = request.PropertyId
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReviewDto>> Update(int id, [FromBody] UpdateReviewDto request, CancellationToken cancellationToken)
    {
        var review = await _sender.Send(new UpdateReviewCommand
        {
            Id = id,
            Rating = request.Rating,
            Comment = request.Comment
        }, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteReviewCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

