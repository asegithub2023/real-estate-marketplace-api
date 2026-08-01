using FluentValidation;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Validators;

public class CreateFavoriteValidator : AbstractValidator<CreateFavoriteDto>
{
    public CreateFavoriteValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");

        RuleFor(x => x.PropertyId)
            .GreaterThan(0).WithMessage("Property ID must be greater than 0.");
    }
}
