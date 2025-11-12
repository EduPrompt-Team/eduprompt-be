
using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly EdupromptV2Context _context;

    public WishlistRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Wishlist?> GetByIdAsync(int id)
    {
        return await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.Package)
            .Include(w => w.StorageTemplate)
            .FirstOrDefaultAsync(w => w.WishlistId == id);
    }

    public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(int UserId)
    {
        return await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.Package)
            .Include(w => w.StorageTemplate)
            .Where(w => w.UserId == UserId)
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync();
    }

    public async Task<Wishlist?> GetUserWishlistItemAsync(int UserId, int templateId)
    {
        // Legacy method - check by PackageId
        return await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.Package)
            .Include(w => w.StorageTemplate)
            .FirstOrDefaultAsync(w => w.UserId == UserId && w.PackageId == templateId);
    }

    public async Task<Wishlist?> GetUserWishlistItemByStorageIdAsync(int userId, int storageId)
    {
        return await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.Package)
            .Include(w => w.StorageTemplate)
            .FirstOrDefaultAsync(w => w.UserId == userId && w.StorageId == storageId);
    }

    public async Task<Wishlist> CreateAsync(Wishlist wishlist)
    {
        wishlist.AddedAt = DateTime.Now;

        await _context.Wishlists.AddAsync(wishlist);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(wishlist.WishlistId) ?? wishlist;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var wishlist = await _context.Wishlists.FindAsync(id);
        if (wishlist == null)
            return false;

        _context.Wishlists.Remove(wishlist);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int UserId, int templateId)
    {
        // Legacy method - check by PackageId
        return await _context.Wishlists
            .AnyAsync(w => w.UserId == UserId && w.PackageId == templateId);
    }

    public async Task<bool> ExistsByStorageIdAsync(int userId, int storageId)
    {
        return await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.StorageId == storageId);
    }

    public async Task<bool> DeleteByStorageIdAsync(int userId, int storageId)
    {
        var wishlist = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.StorageId == storageId);
        
        if (wishlist == null)
            return false;

        _context.Wishlists.Remove(wishlist);
        await _context.SaveChangesAsync();
        return true;
    }
} 
