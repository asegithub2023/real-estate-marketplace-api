using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class UpdateReviewCommand : IRequest<Result<Review, ReviewError>>
{
    public int Id { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
}
