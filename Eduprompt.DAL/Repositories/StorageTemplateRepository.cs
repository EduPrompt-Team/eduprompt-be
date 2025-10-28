
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

        public async Task<IEnumerable<StorageTemplate>> GetPublicAsync(int? packageId, string? grade, string? subject, string? chapter)
        {
            var query = _context.StorageTemplates
                .Include(s => s.User)
                .Include(s => s.Package)
                .Where(s => s.IsPublic)
                .AsQueryable();

            if (packageId.HasValue) query = query.Where(s => s.PackageId == packageId.Value);
            if (!string.IsNullOrWhiteSpace(grade)) query = query.Where(s => s.Grade == grade);
            if (!string.IsNullOrWhiteSpace(subject)) query = query.Where(s => s.Subject == subject);
            if (!string.IsNullOrWhiteSpace(chapter)) query = query.Where(s => s.Chapter == chapter);

            return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
        }

        public async Task<StorageTemplate?> UpdateAsync(StorageTemplate entity)
        {
            _context.ChangeTracker.Clear();
            _context.StorageTemplates.Update(entity);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(entity.StorageId);
        }

        public async Task<bool> SetPublishAsync(int id, bool isPublic)
        {
            var item = await _context.StorageTemplates.FirstOrDefaultAsync(s => s.StorageId == id);
            if (item == null) return false;
            item.IsPublic = isPublic;
            await _context.SaveChangesAsync();
            return true;
        }
} 
