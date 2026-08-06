using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Properties.Commands;

public sealed class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Result<bool, PropertyError>>
{
    private readonly IPropertyRepository _propertyRepository;

    public DeletePropertyCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<bool, PropertyError>> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (property is null)
        {
            return Result.Failure<bool, PropertyError>(PropertyError.NotFound(request.Id));
        }

        await _propertyRepository.DeleteAsync(property, cancellationToken);
        return Result.Success<bool, PropertyError>(true);
    }
}
