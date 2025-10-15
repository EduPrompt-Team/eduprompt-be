using Eduprompt.Domain.DTOs.Wishlist;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IPackageRepository _packageRepository;

    public WishlistService(
        IWishlistRepository wishlistRepository,
        IPackageRepository packageRepository)
    {
        _wishlistRepository = wishlistRepository;
        _packageRepository = packageRepository;
    }

    public async Task<IEnumerable<WishlistDto>> GetByUserIdAsync(int userId)
    {
        var wishlists = await _wishlistRepository.GetByUserIdAsync(userId);
        return wishlists.Select(MapToDto);
    }

    public async Task<WishlistDto?> GetByIdAsync(int wishlistId)
    {
        var wishlist = await _wishlistRepository.GetByIdAsync(wishlistId);
        return wishlist != null ? MapToDto(wishlist) : null;
    }

    public async Task<WishlistDto> CreateAsync(int userId, WishlistCreateDto createDto)
    {
        // Validate package exists
        var package = await _packageRepository.GetByIdAsync(createDto.PackageID);
        if (package == null)
        {
            throw new InvalidOperationException($"Package with ID {createDto.PackageID} not found");
        }

        // Check if already in wishlist
        if (await _wishlistRepository.ExistsAsync(userId, createDto.PackageID))
        {
            throw new InvalidOperationException("Package is already in your wishlist");
        }

        var wishlist = new Wishlist
        {
            UserId = userId,
            PackageID = createDto.PackageID,
            AddedAt = DateTime.UtcNow,
            Notes = createDto.Notes
        };

        var createdWishlist = await _wishlistRepository.CreateAsync(wishlist);
        return MapToDto(createdWishlist);
    }

    public async Task<bool> DeleteAsync(int wishlistId)
    {
        return await _wishlistRepository.DeleteAsync(wishlistId);
    }

    public async Task<bool> IsInWishlistAsync(int userId, int packageId)
    {
        return await _wishlistRepository.ExistsAsync(userId, packageId);
    }

    private static WishlistDto MapToDto(Wishlist wishlist)
    {
        return new WishlistDto
        {
            WishlistId = wishlist.WishlistId,
            UserId = wishlist.UserId,
            PackageID = wishlist.PackageID,
            AddedAt = wishlist.AddedAt,
            Notes = wishlist.Notes,
            UserName = wishlist.User?.FullName,
            PackageName = wishlist.Package?.PackageName,
            PackageDescription = wishlist.Package?.Description,
            PackagePrice = wishlist.Package?.Price
        };
    }
}