using Microsoft.AspNetCore.Http;

namespace RealEstateMarketplace.Infrastructure.Services;

public interface ICloudinaryService
{
    Task<(string ImageUrl, string PublicId)> UploadImageAsync(IFormFile file);
}