using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class CreateReviewCommand : IRequest<Result<ReviewDto, ReviewError>>
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}
