using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class APIKeyRepository : IAPIKeyRepository
{
    private readonly EdupromptContext _context;

    public APIKeyRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<APIKey?> GetByIdAsync(int apiKeyId)
    {
        return await _context.APIKeys
            .Include(k => k.Package)
            .FirstOrDefaultAsync(k => k.APIKeyID == apiKeyId);
    }

    public async Task<IEnumerable<APIKey>> GetByPackageIdAsync(int packageId)
    {
        return await _context.APIKeys
            .Include(k => k.Package)
            .Where(k => k.PackageID == packageId)
            .OrderBy(k => k.CreatedDate)
            .ToListAsync();
    }

    public async Task<APIKey> CreateAsync(APIKey apiKey)
    {
        _context.APIKeys.Add(apiKey);
        await _context.SaveChangesAsync();
        return apiKey;
    }

    public async Task<APIKey> UpdateAsync(APIKey apiKey)
    {
        _context.APIKeys.Update(apiKey);
        await _context.SaveChangesAsync();
        return apiKey;
    }

    public async Task<bool> DeleteAsync(int apiKeyId)
    {
        var apiKey = await _context.APIKeys.FindAsync(apiKeyId);
        if (apiKey == null) return false;

        _context.APIKeys.Remove(apiKey);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int apiKeyId)
    {
        return await _context.APIKeys.AnyAsync(k => k.APIKeyID == apiKeyId);
    }

    public async Task<IEnumerable<APIKey>> GetActiveKeysByPackageIdAsync(int packageId)
    {
        return await _context.APIKeys
            .Include(k => k.Package)
            .Where(k => k.PackageID == packageId && k.Status == "Active")
            .OrderBy(k => k.CreatedDate)
            .ToListAsync();
    }

    public async Task<APIKey?> GetActiveKeyByProviderAsync(string provider)
    {
        return await _context.APIKeys
            .Include(k => k.Package)
            .FirstOrDefaultAsync(k => k.Provider == provider && k.Status == "Active");
    }
}
