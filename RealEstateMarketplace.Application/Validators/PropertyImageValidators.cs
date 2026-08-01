using FluentValidation;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Validators;

public class CreatePropertyImageValidator : AbstractValidator<CreatePropertyImageDto>
{
    public CreatePropertyImageValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.");

        RuleFor(x => x.PropertyId)
            .GreaterThan(0).WithMessage("Property ID must be greater than 0.");
    }
}

public class UpdatePropertyImageValidator : AbstractValidator<UpdatePropertyImageDto>
{
    public UpdatePropertyImageValidator()
    {
        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.");
    }
}
