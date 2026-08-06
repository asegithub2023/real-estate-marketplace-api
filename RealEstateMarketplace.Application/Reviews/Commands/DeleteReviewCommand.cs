using MediatR;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class DeleteReviewCommand : IRequest<bool>
{
    public int Id { get; set; }
}
