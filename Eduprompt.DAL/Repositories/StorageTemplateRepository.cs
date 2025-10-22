
using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class StorageTemplateRepository : IStorageTemplateRepository
{
    private readonly EdupromptV2Context _context;

    public StorageTemplateRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<StorageTemplate?> GetByIdAsync(int id)
    {
        return await _context.StorageTemplates
            .Include(s => s.User)
            .Include(s => s.Package)
            .FirstOrDefaultAsync(s => s.StorageId == id);
    }

    public async Task<IEnumerable<StorageTemplate>> GetByUserIdAsync(int UserId)
    {
        return await _context.StorageTemplates
            .Include(s => s.User)
            .Include(s => s.Package)
            .Where(s => s.UserId == UserId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<StorageTemplate?> GetUserStorageItemAsync(int UserId, int templateId)
    {
        return await _context.StorageTemplates
            .Include(s => s.User)
            .Include(s => s.Package)
            .FirstOrDefaultAsync(s => s.UserId == UserId && s.PackageId == templateId);
    }

    public async Task<StorageTemplate> CreateAsync(StorageTemplate storage)
    {
        storage.CreatedAt = DateTime.Now;

        await _context.StorageTemplates.AddAsync(storage);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(storage.StorageId) ?? storage;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var storage = await _context.StorageTemplates.FindAsync(id);
        if (storage == null)
            return false;

        _context.StorageTemplates.Remove(storage);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int UserId, int templateId)
    {
        return await _context.StorageTemplates
            .AnyAsync(s => s.UserId == UserId && s.PackageId == templateId);
    }
} 
