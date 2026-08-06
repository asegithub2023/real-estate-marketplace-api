using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;

namespace RealEstateMarketplace.Application.PropertyFeatures.Commands;

public sealed class UpdatePropertyFeatureCommandHandler : IRequestHandler<UpdatePropertyFeatureCommand, PropertyFeatureDto?>
{
    private readonly IPropertyFeatureRepository _repository;

    public UpdatePropertyFeatureCommandHandler(IPropertyFeatureRepository repository)
    {
        _repository = repository;
    }

    public async Task<PropertyFeatureDto?> Handle(UpdatePropertyFeatureCommand request, CancellationToken cancellationToken)
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
        return feature.ToDto();
    }
}
