using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class APIKeyRepository : IApikeyRepository
{
    private readonly EdupromptV2Context _context;

    public APIKeyRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Apikey?> GetByIdAsync(int apiKeyId)
    {
        return await _context.Apikeys
            .Include(k => k.Package)
            .FirstOrDefaultAsync(k => k.ApikeyId == apiKeyId);
    }

    public async Task<IEnumerable<Apikey>> GetByPackageIdAsync(int PackageId)
    {
        return await _context.Apikeys
            .Include(k => k.Package)
            .Where(k => k.PackageId == PackageId)
            .ToListAsync();
    }

    public async Task<Apikey> CreateAsync(Apikey apiKey)
    {
        _context.Apikeys.Add(apiKey);
        await _context.SaveChangesAsync();
        return apiKey;
    }

    public async Task<Apikey> UpdateAsync(Apikey apiKey)
    {
        _context.Apikeys.Update(apiKey);
        await _context.SaveChangesAsync();
        return apiKey;
    }

    public async Task<bool> DeleteAsync(int apiKeyId)
    {
        var apiKey = await _context.Apikeys.FindAsync(apiKeyId);
        if (apiKey == null) return false;

        _context.Apikeys.Remove(apiKey);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int apiKeyId)
    {
        return await _context.Apikeys.AnyAsync(k => k.ApikeyId == apiKeyId);
    }

    public async Task<IEnumerable<Apikey>> GetActiveKeysByPackageIdAsync(int PackageId)
    {
        return await _context.Apikeys
            .Include(k => k.Package)
            .Where(k => k.PackageId == PackageId && k.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<Apikey?> GetActiveKeyByProviderAsync(string provider)
    {
        return await _context.Apikeys
            .Include(k => k.Package)
            .FirstOrDefaultAsync(k => k.Apiprovider == provider && k.ExpiresAt > DateTime.UtcNow);
    }
}