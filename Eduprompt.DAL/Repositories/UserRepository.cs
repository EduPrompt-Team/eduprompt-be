using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly EdupromptContext _context;

    public UserRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByGoogleIdAsync(string googleId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.GoogleId == googleId);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        user.CreatedDate = DateTime.Now;
        user.Status = user.Status ?? "Active";
        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(user.UserId) ?? user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedDate = DateTime.Now;
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(user.UserId) ?? user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
} 