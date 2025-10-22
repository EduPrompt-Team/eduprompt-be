using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PackageCategoryRepository : IPackageCategoryRepository
{
    private readonly EdupromptV2Context _context;

    public PackageCategoryRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<PackageCategory?> GetByIdAsync(int PackageCategoryId)
    {
        return await _context.PackageCategories
            .Include(c => c.Packages)
            .FirstOrDefaultAsync(c => c.CategoryId == PackageCategoryId);
    }

    public async Task<IEnumerable<PackageCategory>> GetAllAsync()
    {
        return await _context.PackageCategories
            .Include(c => c.Packages)
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
    }

    public async Task<PackageCategory> CreateAsync(PackageCategory PackageCategory)
    {
        _context.PackageCategories.Add(PackageCategory);
        await _context.SaveChangesAsync();
        return PackageCategory;
    }

    public async Task<PackageCategory> UpdateAsync(PackageCategory PackageCategory)
    {
        _context.PackageCategories.Update(PackageCategory);
        await _context.SaveChangesAsync();
        return PackageCategory;
    }

    public async Task<bool> DeleteAsync(int PackageCategoryId)
    {
        var PackageCategory = await _context.PackageCategories.FindAsync(PackageCategoryId);
        if (PackageCategory == null) return false;

        _context.PackageCategories.Remove(PackageCategory);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int PackageCategoryId)
    {
        return await _context.PackageCategories.AnyAsync(c => c.CategoryId == PackageCategoryId);
    }

    public async Task<IEnumerable<PackageCategory>> GetActiveCategoriesAsync()
    {
        return await _context.PackageCategories
            .Include(c => c.Packages)
            .Where(c => c.CategoryId > 0)
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
    }

    public async Task<int> GetPackageCountByCategoryIdAsync(int CategoryId)
    {
        return await _context.Packages.CountAsync(p => p.CategoryId == CategoryId);
    }
}