using AutoMapper;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
        private readonly IStorageTemplateRepository _templateRepository;
    private readonly IMapper _mapper;

    public CartService(
        ICartRepository cartRepository,
        IStorageTemplateRepository templateRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _templateRepository = templateRepository;
        _mapper = mapper;
    }

    public async Task<CartServiceDto> GetUserCartAsync(int userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        
        // Create cart if not exists
        if (cart == null)
        {
            cart = await _cartRepository.CreateAsync(new Cart { UserId = userId });
        }

        var cartDto = _mapper.Map<CartServiceDto>(cart);
        
        // Calculate total price
        cartDto.TotalPrice = cart.CartDetails?.Sum(cd => cd.SubTotal ?? 0) ?? 0;

        return cartDto;
    }

    public async Task<CartServiceDto> AddItemAsync(int userId, AddCartItemServiceDto itemDto)
    {
        // Validate template exists
        var template = await _templateRepository.GetByIdAsync(itemDto.TemplateId);
        if (template == null)
        {
            throw new InvalidOperationException($"Template with ID {itemDto.TemplateId} not found");
        }

        // Get or create cart
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = await _cartRepository.CreateAsync(new Cart { UserId = userId });
        }

        // Check if item already in cart
        var existingItem = await _cartRepository.GetCartItemByTemplateAsync(cart.CartId, itemDto.TemplateId);
        
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
                TemplateId = itemDto.TemplateId,
                Quantity = itemDto.Quantity,
                UnitPrice = 0 // StorageTemplate doesn't have Price property
            };

            await _cartRepository.AddItemAsync(cartDetail);
        }

        return await GetUserCartAsync(userId);
    }

    public async Task<CartServiceDto> UpdateItemAsync(int userId, int cartDetailId, UpdateCartItemServiceDto itemDto)
    {
        var cartItem = await _cartRepository.GetCartItemAsync(cartDetailId);
        
        if (cartItem == null)
        {
            throw new KeyNotFoundException($"Cart item with ID {cartDetailId} not found");
        }

        // Verify ownership
        var cart = await _cartRepository.GetByIdAsync(cartItem.CartId);
        if (cart == null || cart.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only update items in your own cart");
        }

        cartItem.Quantity = itemDto.Quantity;
        await _cartRepository.UpdateItemAsync(cartItem);

        return await GetUserCartAsync(userId);
    }

    public async Task<bool> RemoveItemAsync(int userId, int cartDetailId)
    {
        var cartItem = await _cartRepository.GetCartItemAsync(cartDetailId);
        
        if (cartItem == null)
            return false;

        // Verify ownership
        var cart = await _cartRepository.GetByIdAsync(cartItem.CartId);
        if (cart == null || cart.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only remove items from your own cart");
        }

        return await _cartRepository.RemoveItemAsync(cartDetailId);
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        return await _cartRepository.ClearCartAsync(userId);
    }
} 
