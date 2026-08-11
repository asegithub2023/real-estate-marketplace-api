using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.PropertyFeatures.Commands;

public sealed class CreatePropertyFeatureCommandHandler : IRequestHandler<CreatePropertyFeatureCommand, PropertyFeature>
{
    private readonly IPropertyFeatureRepository _repository;

    public CreatePropertyFeatureCommandHandler(IPropertyFeatureRepository repository)
    {
        _repository = repository;
    }

    public async Task<PropertyFeature> Handle(CreatePropertyFeatureCommand request, CancellationToken cancellationToken)
    {
        var existingFeature = await _repository.GetByNameAsync(request.Name, cancellationToken);
        if (existingFeature is not null)
        {
            throw new InvalidOperationException("A feature with this name already exists.");
        }

        var feature = new PropertyFeature
        {
            Name = request.Name,
            Icon = request.Icon
        };

        await _repository.AddAsync(feature, cancellationToken);
        return feature;
    }
}
