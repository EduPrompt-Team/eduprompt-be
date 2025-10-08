using Eduprompt.Domain.DTOs.PaymentMethod;
using Eduprompt.Domain.DTOs.Transaction;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreatePaymentMethodValidator : AbstractValidator<CreatePaymentMethodDto>
{
    public CreatePaymentMethodValidator()
    {
        RuleFor(x => x.UserID).GreaterThan(0);
        RuleFor(x => x.MethodType).NotEmpty();
        When(x => x.BankName != null, () => RuleFor(x => x.BankName!).MaximumLength(100));
        When(x => x.AccountNumber != null, () => RuleFor(x => x.AccountNumber!).MaximumLength(100));
    }
}

public class CreateTransactionValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.WalletID).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.TransactionType).NotEmpty();
    }
}


