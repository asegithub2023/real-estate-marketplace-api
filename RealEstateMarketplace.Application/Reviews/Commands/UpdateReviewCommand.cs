using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class UpdateReviewCommand : IRequest<ReviewDto?>
{
    public int Id { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
}
