using Eduprompt.Domain.DTOs.StorageTemplate;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class StorageTemplateCreateValidator : AbstractValidator<StorageTemplateCreateDto>
{
    public StorageTemplateCreateValidator()
    {
        RuleFor(x => x.PackageId).GreaterThan(0);
    }
}


