using Eduprompt.Domain.DTOs.ExpectedOutput;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateExpectedOutputValidator : AbstractValidator<CreateExpectedOutputDto>
{
    public CreateExpectedOutputValidator()
    {
        RuleFor(x => x.PromptInstanceId).GreaterThan(0);
        RuleFor(x => x.OutputName).NotEmpty().MaximumLength(100);
        When(x => x.Status != null, () => RuleFor(x => x.Status!).MaximumLength(50));
        When(x => x.OutputDetails != null, () =>
        {
            RuleForEach(x => x.OutputDetails!).ChildRules(child =>
            {
                child.RuleFor(d => d.OutputSize).GreaterThan(0).When(d => d.OutputSize.HasValue);
                child.RuleFor(d => d.Description).MaximumLength(255).When(d => d.Description != null);
            });
        });
    }
}


