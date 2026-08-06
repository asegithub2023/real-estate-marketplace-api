using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class UpdateReviewCommand : IRequest<Result<ReviewDto, ReviewError>>
{
    public int Id { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
}
