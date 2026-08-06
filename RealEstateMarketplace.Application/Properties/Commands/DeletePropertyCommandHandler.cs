using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Properties.Commands;

public sealed class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;

    public DeletePropertyCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<bool> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (property is null)
        {
            return false;
        }

        await _propertyRepository.DeleteAsync(property, cancellationToken);
        return true;
    }
}
