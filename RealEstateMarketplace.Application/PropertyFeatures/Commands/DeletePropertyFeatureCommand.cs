using MediatR;

namespace RealEstateMarketplace.Application.PropertyFeatures.Commands;

public sealed class DeletePropertyFeatureCommand : IRequest<bool>
{
    public int Id { get; set; }
}
