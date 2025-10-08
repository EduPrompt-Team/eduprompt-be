
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly EdupromptContext _context;

    public WishlistRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<Wishlist?> GetByIdAsync(int id)
    {
        return await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.Template)
            .FirstOrDefaultAsync(w => w.WishlistId == id);
    }

    public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(int userId)
    {
        return await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.Template)
            .Where(w => w.UserId == userId && w.Status == "Active")
            .OrderByDescending(w => w.CreatedDate)
            .ToListAsync();
    }

    public async Task<Wishlist?> GetUserWishlistItemAsync(int userId, int templateId)
    {
        return await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.Template)
            .FirstOrDefaultAsync(w => w.UserId == userId && w.TemplateId == templateId);
    }

    public async Task<Wishlist> CreateAsync(Wishlist wishlist)
    {
        wishlist.CreatedDate = DateTime.Now;
        wishlist.Status = wishlist.Status ?? "Active";

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

    public async Task<bool> ExistsAsync(int userId, int templateId)
    {
        return await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.TemplateId == templateId && w.Status == "Active");
    }
} 