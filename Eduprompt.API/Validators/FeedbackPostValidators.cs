using Eduprompt.Domain.DTOs.Feedback;
using Eduprompt.Domain.DTOs.Post;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateFeedbackValidator : AbstractValidator<CreateFeedbackDto>
{
    public CreateFeedbackValidator()
    {
        RuleFor(x => x.UserID).GreaterThan(0);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        When(x => x.Comment != null, () => RuleFor(x => x.Comment!).MaximumLength(500));
    }
}

public class CreatePostValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.UserID).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty();
    }
}


