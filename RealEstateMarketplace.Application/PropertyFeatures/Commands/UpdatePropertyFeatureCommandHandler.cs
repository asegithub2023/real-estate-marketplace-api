using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.PropertyFeatures.Commands;

public sealed class UpdatePropertyFeatureCommandHandler : IRequestHandler<UpdatePropertyFeatureCommand, PropertyFeature?>
{
    private readonly IPropertyFeatureRepository _repository;

    public UpdatePropertyFeatureCommandHandler(IPropertyFeatureRepository repository)
    {
        _repository = repository;
    }

    public async Task<PropertyFeature?> Handle(UpdatePropertyFeatureCommand request, CancellationToken cancellationToken)
    {
        var feature = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (feature is null)
        {
            return null;
        }

        if (request.Name is not null)
        {
            feature.Name = request.Name;
        }

        if (request.Icon is not null)
        {
            feature.Icon = request.Icon;
        }

        await _repository.UpdateAsync(feature, cancellationToken);
        return feature;
    }
}
