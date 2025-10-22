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

    public async Task<IEnumerable<WishlistDto>> GetByUserIdAsync(int UserId)
    {
        var wishlists = await _wishlistRepository.GetByUserIdAsync(UserId);
        return wishlists.Select(MapToDto);
    }

    public async Task<WishlistDto?> GetByIdAsync(int WishlistId)
    {
        var wishlist = await _wishlistRepository.GetByIdAsync(WishlistId);
        return wishlist != null ? MapToDto(wishlist) : null;
    }

    public async Task<WishlistDto> CreateAsync(int UserId, WishlistCreateDto createDto)
    {
        // Validate package exists
        var package = await _packageRepository.GetByIdAsync(createDto.PackageId);
        if (package == null)
        {
            throw new InvalidOperationException($"Package with ID {createDto.PackageId} not found");
        }

        // Check if already in wishlist
        if (await _wishlistRepository.ExistsAsync(UserId, createDto.PackageId))
        {
            throw new InvalidOperationException("Package is already in your wishlist");
        }

        var wishlist = new Wishlist
        {
            UserId = UserId,
            PackageId = createDto.PackageId,
            AddedAt = DateTime.UtcNow,
            Notes = createDto.Notes
        };

        var createdWishlist = await _wishlistRepository.CreateAsync(wishlist);
        return MapToDto(createdWishlist);
    }

    public async Task<bool> DeleteAsync(int WishlistId)
    {
        return await _wishlistRepository.DeleteAsync(WishlistId);
    }

    public async Task<bool> IsInWishlistAsync(int UserId, int PackageId)
    {
        return await _wishlistRepository.ExistsAsync(UserId, PackageId);
    }

    private static WishlistDto MapToDto(Wishlist wishlist)
    {
        return new WishlistDto
        {
            WishlistId = wishlist.WishlistId,
            UserId = wishlist.UserId,
            PackageId = wishlist.PackageId,
            AddedAt = wishlist.AddedAt,
            Notes = wishlist.Notes,
            UserName = wishlist.User?.FullName,
            PackageName = wishlist.Package?.PackageName,
            PackageDescription = wishlist.Package?.Description,
            PackagePrice = wishlist.Package?.Price
        };
    }
}