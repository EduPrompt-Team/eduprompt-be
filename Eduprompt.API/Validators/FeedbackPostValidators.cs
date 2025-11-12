using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.DTOs.Post;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateFeedbackValidator : AbstractValidator<CreateFeedbackDto>
{
    public CreateFeedbackValidator()
    {
        // PostId or StorageId must be provided (at least one)
        RuleFor(x => x)
            .Must(x => x.PostId.HasValue || x.StorageId.HasValue)
            .WithMessage("PostId or StorageId is required");

        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        When(x => x.Comment != null, () => RuleFor(x => x.Comment!).MaximumLength(5000));
        
        // UserId will be set from token, but validate if provided
        When(x => x.UserId.HasValue, () => 
            RuleFor(x => x.UserId!.Value).GreaterThan(0));
    }
}

public class CreatePostValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty();
    }
}


