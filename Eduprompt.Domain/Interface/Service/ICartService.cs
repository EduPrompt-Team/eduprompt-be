using Eduprompt.Domain.DTOs.Cart;

namespace Eduprompt.Domain.Interface.Service;

public interface ICartService
{
    Task<CartDto?> GetUserCartAsync(int userId);
    Task<CartDto> AddItemAsync(int userId, AddCartItemDto itemDto);
    Task<CartDto> UpdateItemQuantityAsync(int userId, int cartDetailId, int quantity);
    Task<bool> RemoveItemAsync(int userId, int cartDetailId);
    Task<bool> ClearCartAsync(int userId);
} 