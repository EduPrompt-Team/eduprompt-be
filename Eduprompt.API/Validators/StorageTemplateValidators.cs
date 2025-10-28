using Eduprompt.Domain.DTOs.StorageTemplate;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class StorageTemplateCreateValidator : AbstractValidator<StorageTemplateCreateDto>
{
    public StorageTemplateCreateValidator()
    {
        RuleFor(x => x.PackageId).GreaterThan(0);
        RuleFor(x => x.TemplateName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Grade).Must(g => g == null || new[] {"10","11","12"}.Contains(g))
            .WithMessage("Grade must be one of: 10, 11, 12");
        RuleFor(x => x.Subject).MaximumLength(50);
        RuleFor(x => x.Chapter).MaximumLength(100);
    }
}

public class StorageTemplateUpdateValidator : AbstractValidator<StorageTemplateUpdateDto>
{
    public StorageTemplateUpdateValidator()
    {
        RuleFor(x => x.TemplateName).MaximumLength(200);
        RuleFor(x => x.Grade).Must(g => g == null || new[] {"10","11","12"}.Contains(g))
            .WithMessage("Grade must be one of: 10, 11, 12");
        RuleFor(x => x.Subject).MaximumLength(50);
        RuleFor(x => x.Chapter).MaximumLength(100);
    }
}


