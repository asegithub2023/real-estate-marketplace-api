using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class CreateReviewCommand : IRequest<ReviewDto>
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}
