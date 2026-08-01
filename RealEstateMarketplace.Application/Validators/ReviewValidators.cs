using FluentValidation;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Validators;

public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");

        RuleFor(x => x.PropertyId)
            .GreaterThan(0).WithMessage("Property ID must be greater than 0.");
    }
}

public class UpdateReviewValidator : AbstractValidator<UpdateReviewDto>
{
    public UpdateReviewValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).When(x => x.Rating.HasValue).WithMessage("Rating must be between 1 and 5.");
    }
}
