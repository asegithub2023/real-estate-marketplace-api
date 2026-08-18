using FluentValidation;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Validators;

public class CreatePropertyValidator : AbstractValidator<CreatePropertyDto>
{
    public CreatePropertyValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title must not exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(250).WithMessage("Address must not exceed 250 characters.");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0).WithMessage("Bedrooms must be greater than or equal to 0.");

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0).WithMessage("Bathrooms must be greater than or equal to 0.");

        RuleFor(x => x.Rooms)
            .GreaterThanOrEqualTo(0).WithMessage("Rooms must be greater than or equal to 0.");

        RuleFor(x => x.Area)
            .GreaterThan(0).WithMessage("Area must be greater than 0.");

        RuleFor(x => x.OwnerId)
            .GreaterThan(0).WithMessage("Owner ID must be greater than 0.");

       RuleFor(x => x.Images)
    .NotEmpty()
    .WithMessage("At least one image is required.");

RuleForEach(x => x.Images)
    .Must(file => file is not null && file.Length > 0)
    .WithMessage("Image file cannot be empty.")
    .Must(file =>
        file.ContentType == "image/jpeg" ||
        file.ContentType == "image/png" ||
        file.ContentType == "image/webp")
    .WithMessage("Only JPEG, PNG, and WebP images are allowed.");
    }
}

public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyDto>
{
    public UpdatePropertyValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(150).WithMessage("Title must not exceed 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).When(x => x.Price.HasValue).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(250).WithMessage("Address must not exceed 250 characters.");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0).When(x => x.Bedrooms.HasValue).WithMessage("Bedrooms must be greater than or equal to 0.");

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0).When(x => x.Bathrooms.HasValue).WithMessage("Bathrooms must be greater than or equal to 0.");

        RuleFor(x => x.Rooms)
            .GreaterThanOrEqualTo(0).When(x => x.Rooms.HasValue).WithMessage("Rooms must be greater than or equal to 0.");

        RuleFor(x => x.Area)
            .GreaterThan(0).When(x => x.Area.HasValue).WithMessage("Area must be greater than 0.");
    }
}
