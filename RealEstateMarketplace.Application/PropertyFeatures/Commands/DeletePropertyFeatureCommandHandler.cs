using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.PropertyFeatures.Commands;

public sealed class DeletePropertyFeatureCommandHandler : IRequestHandler<DeletePropertyFeatureCommand, bool>
{
    private readonly IPropertyFeatureRepository _repository;

    public DeletePropertyFeatureCommandHandler(IPropertyFeatureRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeletePropertyFeatureCommand request, CancellationToken cancellationToken)
    {
        var feature = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (feature is null)
        {
            return false;
        }

        await _repository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
