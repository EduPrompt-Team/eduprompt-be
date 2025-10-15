using Eduprompt.Domain.DTOs.PaymentMethod;
using Eduprompt.Domain.DTOs.Transaction;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreatePaymentMethodValidator : AbstractValidator<CreatePaymentMethodDto>
{
    public CreatePaymentMethodValidator()
    {
        RuleFor(x => x.MethodName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Provider).NotEmpty().MaximumLength(50);
        When(x => x.ProcessingFee.HasValue, () => RuleFor(x => x.ProcessingFee!.Value).GreaterThanOrEqualTo(0));
    }
}

public class CreateTransactionValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.WalletID).GreaterThan(0);
        RuleFor(x => x.PaymentMethodID).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.TransactionType).NotEmpty();
    }
}


