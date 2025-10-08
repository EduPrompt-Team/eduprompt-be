using Eduprompt.Domain.DTOs.User;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateUserValidator : AbstractValidator<UserCreateDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FullName).NotEmpty();
    }
}

public class UpdateUserValidator : AbstractValidator<UserUpdateDto>
{
    public UpdateUserValidator()
    {
        // UserUpdateDto không có Email; chỉ validate các trường hiện diện
        When(x => x.FullName != null, () => RuleFor(x => x.FullName!).NotEmpty());
    }
}


