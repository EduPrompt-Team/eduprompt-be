using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly EdupromptContext _context;

    public PackageRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<Package?> GetByIdAsync(int packageId)
    {
        return await _context.Packages
            .Include(p => p.PackageCategory)
            .Include(p => p.PackageDetails)
            .Include(p => p.APIKeys)
            .FirstOrDefaultAsync(p => p.PackageID == packageId);
    }

    public async Task<IEnumerable<Package>> GetAllAsync()
    {
        return await _context.Packages
            .Include(p => p.PackageCategory)
            .Include(p => p.PackageDetails)
            .OrderBy(p => p.PackageName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Packages
            .Include(p => p.PackageCategory)
            .Include(p => p.PackageDetails)
            .Where(p => p.CategoryID == categoryId)
            .OrderBy(p => p.PackageName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetActivePackagesAsync()
    {
        return await _context.Packages
            .Include(p => p.PackageCategory)
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
            .Include(p => p.PackageCategory)
            .Include(p => p.PackageDetails)
            .FirstOrDefaultAsync(p => p.PackageID == package.PackageID) ?? package;
    }

    public async Task<Package> UpdateAsync(Package package)
    {
        _context.Packages.Update(package);
        await _context.SaveChangesAsync();
        return package;
    }

    public async Task<bool> DeleteAsync(int packageId)
    {
        var package = await _context.Packages.FindAsync(packageId);
        if (package == null) return false;

        _context.Packages.Remove(package);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int packageId)
    {
        return await _context.Packages.AnyAsync(p => p.PackageID == packageId);
    }

    public async Task<IEnumerable<Package>> SearchAsync(string searchTerm)
    {
        return await _context.Packages
            .Include(p => p.PackageCategory)
            .Include(p => p.PackageDetails)
            .Where(p => p.PackageName.Contains(searchTerm) || 
                       (p.Description != null && p.Description.Contains(searchTerm)))
            .OrderBy(p => p.PackageName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Package>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Packages
            .Include(p => p.PackageCategory)
            .Include(p => p.PackageDetails)
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }
}
