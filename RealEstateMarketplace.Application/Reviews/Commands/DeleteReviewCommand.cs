using MediatR;
using RealEstateMarketplace.Application.Common;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class DeleteReviewCommand : IRequest<Result<bool, ReviewError>>
{
    public int Id { get; set; }
}
