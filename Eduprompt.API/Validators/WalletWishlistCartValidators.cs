using Eduprompt.Domain.DTOs.Wallet;
using Eduprompt.Domain.DTOs.Wishlist;
using Eduprompt.Domain.DTOs.Cart;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateWalletValidator : AbstractValidator<CreateWalletDto>
{
    public CreateWalletValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
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
        // StorageId is required (for prompt templates)
        RuleFor(x => x.StorageId).GreaterThan(0).WithMessage("Storage ID is required and must be greater than 0");
        
        // PackageId is optional (for backward compatibility)
        RuleFor(x => x.PackageId).GreaterThan(0).When(x => x.PackageId.HasValue)
            .WithMessage("Package ID must be greater than 0 if provided");
        
        // Notes validation
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes != null)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}

public class AddCartItemValidator : AbstractValidator<AddCartItemDto>
{
    public AddCartItemValidator()
    {
        RuleFor(x => x.PackageId).GreaterThan(0);
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


