using Eduprompt.Domain.DTOs.Cart;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IPackageRepository _packageRepository;

    public CartService(
        ICartRepository cartRepository,
        IPackageRepository packageRepository)
    {
        _cartRepository = cartRepository;
        _packageRepository = packageRepository;
    }

    public async Task<CartDto?> GetUserCartAsync(int userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null) return null;

        var cartDto = MapToDto(cart);
        
        // Calculate total price
        cartDto.TotalPrice = cart.CartDetails?.Sum(cd => cd.UnitPrice * cd.Quantity) ?? 0;

        return cartDto;
    }

    public async Task<CartDto> AddItemAsync(int userId, AddCartItemDto itemDto)
    {
        // Validate package exists
        var package = await _packageRepository.GetByIdAsync(itemDto.PackageID);
        if (package == null)
        {
            throw new InvalidOperationException($"Package with ID {itemDto.PackageID} not found");
        }

        // Get or create cart
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                TotalItem = 0,
                CreatedDate = DateTime.UtcNow
            };
            cart = await _cartRepository.CreateAsync(cart);
        }

        // Check if item already exists in cart
        var existingItem = await _cartRepository.GetCartItemByPackageAsync(cart.CartId, itemDto.PackageID);
        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += itemDto.Quantity;
            await _cartRepository.UpdateItemAsync(existingItem);
        }
        else
        {
            // Add new item
            var cartDetail = new CartDetail
            {
                CartId = cart.CartId,
                PackageID = itemDto.PackageID,
                Quantity = itemDto.Quantity,
                UnitPrice = package.Price,
                AddedDate = DateTime.UtcNow
            };
            await _cartRepository.AddItemAsync(cartDetail);
        }

        // Update cart total items
        cart.TotalItem = cart.CartDetails?.Sum(cd => cd.Quantity) ?? 0;
        cart.UpdatedDate = DateTime.UtcNow;
        await _cartRepository.UpdateAsync(cart);

        return await GetUserCartAsync(userId) ?? new CartDto();
    }

    public async Task<CartDto> UpdateItemQuantityAsync(int userId, int cartDetailId, int quantity)
    {
        var cartDetail = await _cartRepository.GetCartItemAsync(cartDetailId);
        if (cartDetail == null)
        {
            throw new InvalidOperationException("Cart item not found");
        }

        // Verify ownership
        var cart = await _cartRepository.GetByIdAsync(cartDetail.CartId);
        if (cart?.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only modify your own cart");
        }

        if (quantity <= 0)
        {
            await _cartRepository.RemoveItemAsync(cartDetailId);
        }
        else
        {
            cartDetail.Quantity = quantity;
            await _cartRepository.UpdateItemAsync(cartDetail);
        }

        // Update cart total items
        cart.TotalItem = cart.CartDetails?.Sum(cd => cd.Quantity) ?? 0;
        cart.UpdatedDate = DateTime.UtcNow;
        await _cartRepository.UpdateAsync(cart);

        return await GetUserCartAsync(userId) ?? new CartDto();
    }

    public async Task<bool> RemoveItemAsync(int userId, int cartDetailId)
    {
        var cartDetail = await _cartRepository.GetCartItemAsync(cartDetailId);
        if (cartDetail == null) return false;

        // Verify ownership
        var cart = await _cartRepository.GetByIdAsync(cartDetail.CartId);
        if (cart?.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only modify your own cart");
        }

        var result = await _cartRepository.RemoveItemAsync(cartDetailId);
        
        if (result)
        {
            // Update cart total items
            cart.TotalItem = cart.CartDetails?.Sum(cd => cd.Quantity) ?? 0;
            cart.UpdatedDate = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);
        }

        return result;
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        return await _cartRepository.ClearCartAsync(userId);
    }

    private static CartDto MapToDto(Cart cart)
    {
        var updated = cart.UpdatedDate ?? cart.CreatedDate;
        return new CartDto
        {
            CartId = cart.CartId,
            UserId = cart.UserId,
            TotalItems = cart.TotalItem ?? 0,
            CreatedDate = cart.CreatedDate ?? DateTime.UtcNow,
            UpdatedDate = updated,
            Items = cart.CartDetails?.Select(cd => new CartItemDto
            {
                CartDetailId = cd.CartDetailId,
                CartId = cd.CartId,
                PackageID = cd.PackageID,
                Quantity = cd.Quantity,
                UnitPrice = cd.UnitPrice,
                AddedDate = cd.AddedDate,
                PackageName = cd.Package?.PackageName,
                PackageDescription = cd.Package?.Description
            }).ToList()
        };
    }
}