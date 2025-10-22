using Eduprompt.Domain.DTOs.TemplateArchitecture;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateTemplateArchitectureValidator : AbstractValidator<CreateTemplateArchitectureDto>
{
    public CreateTemplateArchitectureValidator()
    {
        RuleFor(x => x.PromptInstanceId).GreaterThan(0);
        RuleFor(x => x.ArchitectureName).NotEmpty().MaximumLength(100);
        When(x => x.Description != null, () => RuleFor(x => x.Description!).MaximumLength(500));
        When(x => x.Status != null, () => RuleFor(x => x.Status!).MaximumLength(50));
    }
}


