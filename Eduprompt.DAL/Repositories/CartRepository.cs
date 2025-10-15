using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class CartRepository : ICartRepository
{
    private readonly EdupromptContext _context;

    public CartRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _context.Carts
            .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Package)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart?> GetByIdAsync(int cartId)
    {
        return await _context.Carts
            .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Package)
            .FirstOrDefaultAsync(c => c.CartId == cartId);
    }

    public async Task<Cart> CreateAsync(Cart cart)
    {
        cart.CreatedDate = DateTime.Now;
        cart.TotalItem = 0;

        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(cart.CartId) ?? cart;
    }

    public async Task<Cart> UpdateAsync(Cart cart)
    {
        cart.UpdatedDate = DateTime.Now;

        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(cart.CartId) ?? cart;
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart == null) return false;

        _context.CartDetails.RemoveRange(cart.CartDetails);
        
        cart.TotalItem = 0;
        cart.UpdatedDate = DateTime.Now;
        
        await _context.SaveChangesAsync();
        return true;
    }

    // Cart Items Methods
    public async Task<CartDetail?> GetCartItemAsync(int cartDetailId)
    {
        return await _context.CartDetails
            .Include(cd => cd.Package)
            .FirstOrDefaultAsync(cd => cd.CartDetailId == cartDetailId);
    }

    public async Task<CartDetail?> GetCartItemByPackageAsync(int cartId, int packageId)
    {
        return await _context.CartDetails
            .Include(cd => cd.Package)
            .FirstOrDefaultAsync(cd => cd.CartId == cartId && cd.PackageID == packageId);
    }

    public async Task<CartDetail> AddItemAsync(CartDetail cartDetail)
    {
        cartDetail.AddedDate = DateTime.Now;
        // CartDetailId is auto-generated

        await _context.CartDetails.AddAsync(cartDetail);
        
        // Update cart total items
        var cart = await _context.Carts.FindAsync(cartDetail.CartId);
        if (cart != null)
        {
            cart.TotalItem = (cart.TotalItem ?? 0) + 1;
            cart.UpdatedDate = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        return await GetCartItemAsync(cartDetail.CartDetailId) ?? cartDetail;
    }

    public async Task<CartDetail> UpdateItemAsync(CartDetail cartDetail)
    {
        var cart = await _context.Carts.FindAsync(cartDetail.CartId);
        if (cart != null)
        {
            cart.UpdatedDate = DateTime.Now;
        }

        _context.CartDetails.Update(cartDetail);
        await _context.SaveChangesAsync();

        return await GetCartItemAsync(cartDetail.CartDetailId) ?? cartDetail;
    }

    public async Task<bool> RemoveItemAsync(int cartDetailId)
    {
        var cartDetail = await _context.CartDetails.FindAsync(cartDetailId);
        if (cartDetail == null) return false;

        // Update cart total items
        var cart = await _context.Carts.FindAsync(cartDetail.CartId);
        if (cart != null && (cart.TotalItem ?? 0) > 0)
        {
            cart.TotalItem = (cart.TotalItem ?? 0) - 1;
            cart.UpdatedDate = DateTime.Now;
        }

        _context.CartDetails.Remove(cartDetail);
        await _context.SaveChangesAsync();
        
        return true;
    }
} 