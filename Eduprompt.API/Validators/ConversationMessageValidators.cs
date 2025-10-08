using Eduprompt.Domain.DTOs.Conversation;
using Eduprompt.Domain.DTOs.Message;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateConversationValidator : AbstractValidator<CreateConversationDto>
{
    public CreateConversationValidator()
    {
        RuleFor(x => x.UserID).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public class CreateMessageValidator : AbstractValidator<CreateMessageDto>
{
    public CreateMessageValidator()
    {
        RuleFor(x => x.ConversationID).GreaterThan(0);
        RuleFor(x => x.Content).NotEmpty();
    }
}


