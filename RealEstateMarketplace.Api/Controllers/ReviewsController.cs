using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("property/{propertyId:int}")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetByPropertyId(int propertyId, CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetByPropertyIdAsync(propertyId, cancellationToken);
        return Ok(reviews);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReviewDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var review = await _reviewService.GetByIdAsync(id, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var review = await _reviewService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReviewDto>> Update(int id, [FromBody] UpdateReviewDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var review = await _reviewService.UpdateAsync(id, request, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _reviewService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
