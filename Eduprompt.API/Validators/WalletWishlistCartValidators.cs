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
        RuleFor(x => x.PackageID).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes != null);
    }
}

public class AddCartItemValidator : AbstractValidator<AddCartItemDto>
{
    public AddCartItemValidator()
    {
        RuleFor(x => x.PackageID).GreaterThan(0);
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


