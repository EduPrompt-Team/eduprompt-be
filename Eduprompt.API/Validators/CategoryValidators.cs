using Eduprompt.Domain.DTOs.PackageCategory;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateCategoryValidator : AbstractValidator<CreatePackageCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(100);
        When(x => x.Description != null, () => RuleFor(x => x.Description!).MaximumLength(500));
        When(x => x.Status != null, () => RuleFor(x => x.Status!).MaximumLength(50));
    }
}

public class UpdateCategoryValidator : AbstractValidator<CreatePackageCategoryDto>
{
    public UpdateCategoryValidator()
    {
        When(x => x.CategoryName != null, () => RuleFor(x => x.CategoryName!).NotEmpty().MaximumLength(100));
        When(x => x.Description != null, () => RuleFor(x => x.Description!).MaximumLength(500));
        When(x => x.Status != null, () => RuleFor(x => x.Status!).MaximumLength(50));
    }
}


