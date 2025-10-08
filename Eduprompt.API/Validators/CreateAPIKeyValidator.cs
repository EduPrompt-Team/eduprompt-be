using Eduprompt.Domain.DTOs.APIKey;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateAPIKeyValidator : AbstractValidator<CreateAPIKeyDto>
{
    public CreateAPIKeyValidator()
    {
        RuleFor(x => x.PackageID).GreaterThan(0);
        RuleFor(x => x.KeyName).NotEmpty().MaximumLength(100);
        When(x => x.KeyValue != null, () => RuleFor(x => x.KeyValue!).MaximumLength(500));
        When(x => x.Provider != null, () => RuleFor(x => x.Provider!).MaximumLength(50));
    }
}


