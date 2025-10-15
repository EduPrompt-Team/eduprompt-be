using Eduprompt.Domain.DTOs.PromptInstance;
using Eduprompt.Domain.DTOs.PromptInstanceDetail;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreatePromptInstanceValidator : AbstractValidator<CreatePromptInstanceDto>
{
    public CreatePromptInstanceValidator()
    {
        RuleFor(x => x.UserID).GreaterThan(0);
        RuleFor(x => x.PromptName).NotEmpty().MaximumLength(200);
    }
}

public class UpdatePromptInstanceValidator : AbstractValidator<UpdatePromptInstanceDto>
{
    public UpdatePromptInstanceValidator()
    {
        When(x => x.PromptName != null, () => RuleFor(x => x.PromptName!).NotEmpty().MaximumLength(200));
    }
}

public class CreatePromptInstanceDetailValidator : AbstractValidator<CreatePromptInstanceDetailDto>
{
    public CreatePromptInstanceDetailValidator()
    {
        RuleFor(x => x.InstanceID).GreaterThan(0);
        RuleFor(x => x.FieldName).NotEmpty().MaximumLength(100);
        When(x => x.FieldType != null, () => RuleFor(x => x.FieldType!).MaximumLength(50));
        When(x => x.OrderIndex.HasValue, () => RuleFor(x => x.OrderIndex!.Value).GreaterThanOrEqualTo(0));
    }
}


