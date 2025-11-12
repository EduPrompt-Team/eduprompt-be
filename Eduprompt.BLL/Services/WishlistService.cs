using Eduprompt.Domain.DTOs.Wishlist;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IPackageRepository _packageRepository;
    private readonly IStorageTemplateRepository _storageTemplateRepository;

    public WishlistService(
        IWishlistRepository wishlistRepository,
        IPackageRepository packageRepository,
        IStorageTemplateRepository storageTemplateRepository)
    {
        _wishlistRepository = wishlistRepository;
        _packageRepository = packageRepository;
        _storageTemplateRepository = storageTemplateRepository;
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
        // Validate StorageTemplate exists (required)
        var storageTemplate = await _storageTemplateRepository.GetByIdAsync(createDto.StorageId);
        if (storageTemplate == null)
        {
            throw new InvalidOperationException($"StorageTemplate with ID {createDto.StorageId} not found");
        }

        // Validate Package exists (optional, for backward compatibility)
        if (createDto.PackageId.HasValue)
        {
            var package = await _packageRepository.GetByIdAsync(createDto.PackageId.Value);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {createDto.PackageId} not found");
            }
        }

        // Check if StorageTemplate already in wishlist
        if (await _wishlistRepository.ExistsByStorageIdAsync(UserId, createDto.StorageId))
        {
            throw new InvalidOperationException("StorageTemplate is already in your wishlist");
        }

        var wishlist = new Wishlist
        {
            UserId = UserId,
            PackageId = createDto.PackageId ?? 0,  // Use 0 as sentinel for NULL (DbContext will convert to NULL in DB)
            StorageId = createDto.StorageId,   // Required
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

    public async Task<bool> DeleteByStorageIdAsync(int userId, int storageId)
    {
        return await _wishlistRepository.DeleteByStorageIdAsync(userId, storageId);
    }

    public async Task<bool> IsInWishlistAsync(int UserId, int PackageId)
    {
        // Legacy method - check by PackageId
        return await _wishlistRepository.ExistsAsync(UserId, PackageId);
    }

    public async Task<bool> IsInWishlistByStorageIdAsync(int userId, int storageId)
    {
        return await _wishlistRepository.ExistsByStorageIdAsync(userId, storageId);
    }

    private static WishlistDto MapToDto(Wishlist wishlist)
    {
        return new WishlistDto
        {
            WishlistId = wishlist.WishlistId,
            UserId = wishlist.UserId,
            PackageId = wishlist.PackageId == 0 ? null : wishlist.PackageId,  // Convert 0 (sentinel) to null
            StorageId = wishlist.StorageId,   // New field
            AddedAt = wishlist.AddedAt,
            Notes = wishlist.Notes,
            
            // User info
            UserName = wishlist.User?.FullName,
            
            // Package info (legacy, for backward compatibility)
            PackageName = wishlist.Package?.PackageName,
            PackageDescription = wishlist.Package?.Description,
            PackagePrice = wishlist.Package?.Price,
            
            // StorageTemplate info (prompt template)
            TemplateName = wishlist.StorageTemplate?.TemplateName,
            TemplateContent = wishlist.StorageTemplate?.TemplateContent,
            Grade = wishlist.StorageTemplate?.Grade,
            Subject = wishlist.StorageTemplate?.Subject,
            Chapter = wishlist.StorageTemplate?.Chapter,
            IsPublic = wishlist.StorageTemplate?.IsPublic,
            TemplateCreatedAt = wishlist.StorageTemplate?.CreatedAt
        };
    }
}