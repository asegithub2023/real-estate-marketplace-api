using FluentValidation;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Validators;

public class CreatePropertyFeatureValidator : AbstractValidator<CreatePropertyFeatureDto>
{
    public CreatePropertyFeatureValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Feature name is required.")
            .MaximumLength(100).WithMessage("Feature name must not exceed 100 characters.");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("Icon must not exceed 100 characters.");
    }
}

public class UpdatePropertyFeatureValidator : AbstractValidator<UpdatePropertyFeatureDto>
{
    public UpdatePropertyFeatureValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Feature name must not exceed 100 characters.");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("Icon must not exceed 100 characters.");
    }
}
