using Eduprompt.Domain.DTOs.Wallet;
using Eduprompt.Domain.DTOs.Wishlist;
using Eduprompt.Domain.DTOs.Cart;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateWalletValidator : AbstractValidator<CreateWalletDto>
{
    public CreateWalletValidator()
    {
        RuleFor(x => x.UserID).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
    }
}

public class UpdateWalletValidator : AbstractValidator<UpdateWalletDto>
{
    public UpdateWalletValidator()
    {
        RuleFor(x => x.Balance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
    }
}

public class CreateWishlistValidator : AbstractValidator<WishlistCreateDto>
{
    public CreateWishlistValidator()
    {
        RuleFor(x => x.TemplateId).GreaterThan(0);
        RuleFor(x => x.WishlistName).MaximumLength(100).When(x => x.WishlistName != null);
    }
}

public class AddCartItemValidator : AbstractValidator<AddCartItemDto>
{
    public AddCartItemValidator()
    {
        RuleFor(x => x.TemplateId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}


