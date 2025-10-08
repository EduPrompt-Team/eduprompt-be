
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class StorageTemplateRepository : IStorageTemplateRepository
{
    private readonly EdupromptContext _context;

    public StorageTemplateRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<StorageTemplate?> GetByIdAsync(int id)
    {
        return await _context.StorageTemplates
            .Include(s => s.User)
            // .Include(s => s.Template) // Removed - Template navigation property deleted
            .FirstOrDefaultAsync(s => s.StorageId == id);
    }

    public async Task<IEnumerable<StorageTemplate>> GetByUserIdAsync(int userId)
    {
        return await _context.StorageTemplates
            .Include(s => s.User)
            // .Include(s => s.Template) // Removed - Template navigation property deleted
            .Where(s => s.UserId == userId && s.Status == "Active")
            .OrderByDescending(s => s.UploadDate)
            .ToListAsync();
    }

    public async Task<StorageTemplate?> GetUserStorageItemAsync(int userId, int templateId)
    {
        return await _context.StorageTemplates
            .Include(s => s.User)
            // .Include(s => s.Template) // Removed - Template navigation property deleted
            .FirstOrDefaultAsync(s => s.UserId == userId && s.TemplateId == templateId);
    }

    public async Task<StorageTemplate> CreateAsync(StorageTemplate storage)
    {
        storage.UploadDate = DateTime.Now;
        storage.Status = storage.Status ?? "Active";

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

    public async Task<bool> ExistsAsync(int userId, int templateId)
    {
        return await _context.StorageTemplates
            .AnyAsync(s => s.UserId == userId && s.TemplateId == templateId && s.Status == "Active");
    }
} 