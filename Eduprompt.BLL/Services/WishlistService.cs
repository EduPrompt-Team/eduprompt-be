using AutoMapper;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
        private readonly IStorageTemplateRepository _templateRepository;
    private readonly IMapper _mapper;

    public WishlistService(
        IWishlistRepository wishlistRepository,
        IStorageTemplateRepository templateRepository,
        IMapper mapper)
    {
        _wishlistRepository = wishlistRepository;
        _templateRepository = templateRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WishlistServiceDto>> GetUserWishlistAsync(int userId)
    {
        var wishlists = await _wishlistRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<WishlistServiceDto>>(wishlists);
    }

    public async Task<WishlistServiceDto> AddToWishlistAsync(int userId, WishlistCreateServiceDto wishlistDto)
    {
        // Validate template exists
        if (!await _templateRepository.ExistsAsync(wishlistDto.TemplateId, userId))
        {
            throw new InvalidOperationException($"Template with ID {wishlistDto.TemplateId} not found");
        }

        // Check if already in wishlist
        if (await _wishlistRepository.ExistsAsync(userId, wishlistDto.TemplateId))
        {
            throw new InvalidOperationException("Template is already in your wishlist");
        }

        var wishlist = new Wishlist
        {
            UserId = userId,
            TemplateId = wishlistDto.TemplateId,
            WishlistName = wishlistDto.WishlistName ?? "My Favorites"
        };

        var createdWishlist = await _wishlistRepository.CreateAsync(wishlist);
        return _mapper.Map<WishlistServiceDto>(createdWishlist);
    }

    public async Task<bool> RemoveFromWishlistAsync(int id, int userId)
    {
        var wishlist = await _wishlistRepository.GetByIdAsync(id);
        
        if (wishlist == null)
            return false;

        // Only the owner can remove
        if (wishlist.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only remove items from your own wishlist");
        }

        return await _wishlistRepository.DeleteAsync(id);
    }

    public async Task<bool> IsInWishlistAsync(int userId, int templateId)
    {
        return await _wishlistRepository.ExistsAsync(userId, templateId);
    }
} 
