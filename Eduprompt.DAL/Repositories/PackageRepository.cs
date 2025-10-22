using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly EdupromptV2Context _context;

    public PackageRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Package?> GetByIdAsync(int PackageId)
    {
        return await _context.Packages
            .Include(p => p.Category)
            .Include(p => p.PackageDetails)
            .Include(p => p.Apikeys)
            .FirstOrDefaultAsync(p => p.PackageId == PackageId);
    }

    public async Task<IEnumerable<Package>> GetAllAsync()
    {
        return await _context.Packages
            .Include(p => p.Category)
            .Include(p => p.PackageDetails)
            .OrderBy(p => p.PackageName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetByCategoryIdAsync(int CategoryId)
    {
        return await _context.Packages
            .Include(p => p.Category)
            .Include(p => p.PackageDetails)
            .Where(p => p.CategoryId == CategoryId)
            .OrderBy(p => p.PackageName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetActivePackagesAsync()
    {
        return await _context.Packages
            .Include(p => p.Category)
            .Include(p => p.PackageDetails)
            .Where(p => p.IsActive == true)
            .OrderBy(p => p.PackageName)
            .ToListAsync();
    }

    public async Task<Package> CreateAsync(Package package)
    {
        _context.Packages.Add(package);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Packages
            .Include(p => p.Category)
            .Include(p => p.PackageDetails)
            .FirstOrDefaultAsync(p => p.PackageId == package.PackageId) ?? package;
    }

    public async Task<Package> UpdateAsync(Package package)
    {
        _context.Packages.Update(package);
        await _context.SaveChangesAsync();
        return package;
    }

    public async Task<bool> DeleteAsync(int PackageId)
    {
        var package = await _context.Packages.FindAsync(PackageId);
        if (package == null) return false;

        _context.Packages.Remove(package);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int PackageId)
    {
        return await _context.Packages.AnyAsync(p => p.PackageId == PackageId);
    }

    public async Task<IEnumerable<Package>> SearchAsync(string searchTerm)
    {
        return await _context.Packages
            .Include(p => p.Category)
            .Include(p => p.PackageDetails)
            .Where(p => p.PackageName.Contains(searchTerm) || 
                       (p.Description != null && p.Description.Contains(searchTerm)))
            .OrderBy(p => p.PackageName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Packages
            .Include(p => p.Category)
            .Include(p => p.PackageDetails)
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }
}
