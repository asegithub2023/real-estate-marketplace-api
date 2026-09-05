using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(
        User user,
        IList<string> roles);
}
