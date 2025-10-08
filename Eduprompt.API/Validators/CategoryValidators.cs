using Eduprompt.Domain.DTOs.Category;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateCategoryValidator : AbstractValidator<CategoryCreateDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(100);
        When(x => x.Description != null, () => RuleFor(x => x.Description!).MaximumLength(500));
    }
}

public class UpdateCategoryValidator : AbstractValidator<CategoryUpdateDto>
{
    public UpdateCategoryValidator()
    {
        When(x => x.CategoryName != null, () => RuleFor(x => x.CategoryName!).NotEmpty().MaximumLength(100));
        When(x => x.Description != null, () => RuleFor(x => x.Description!).MaximumLength(500));
    }
}


