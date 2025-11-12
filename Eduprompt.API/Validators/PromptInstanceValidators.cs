using Eduprompt.Domain.DTOs.PromptInstance;
using Eduprompt.Domain.DTOs.PromptInstanceDetail;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreatePromptInstanceValidator : AbstractValidator<CreatePromptInstanceDto>
{
    public CreatePromptInstanceValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId is required and must be greater than 0");
        
        RuleFor(x => x.PromptName).NotEmpty().WithMessage("PromptName is required")
            .MaximumLength(200).WithMessage("PromptName cannot exceed 200 characters");
        
        // PackageId is optional - can be null or 0
        // If provided, must be > 0
        When(x => x.PackageId.HasValue, () => 
        {
            RuleFor(x => x.PackageId!.Value)
                .GreaterThan(0)
                .WithMessage("PackageId must be greater than 0 if provided");
        });
        
        // StorageId is optional - can be null or 0
        // If provided, must be > 0
        When(x => x.StorageId.HasValue, () => 
        {
            RuleFor(x => x.StorageId!.Value)
                .GreaterThan(0)
                .WithMessage("StorageId must be greater than 0 if provided");
        });
        
        // At least one of PackageId or StorageId should be provided (or both can be null/0 for instances without package)
        // This is handled in service layer, not in validation
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
        RuleFor(x => x.PromptInstanceId).GreaterThan(0);
        RuleFor(x => x.FieldName).NotEmpty().MaximumLength(100);
        When(x => x.FieldType != null, () => RuleFor(x => x.FieldType!).MaximumLength(50));
        When(x => x.OrderIndex.HasValue, () => RuleFor(x => x.OrderIndex!.Value).GreaterThanOrEqualTo(0));
    }
}


