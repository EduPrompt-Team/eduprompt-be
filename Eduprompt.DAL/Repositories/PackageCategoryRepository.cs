using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class PackageCategoryRepository : IPackageCategoryRepository
{
    private readonly EdupromptContext _context;

    public PackageCategoryRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<PackageCategory?> GetByIdAsync(int categoryId)
    {
        return await _context.PackageCategories
            .Include(c => c.Packages)
            .FirstOrDefaultAsync(c => c.CategoryID == categoryId);
    }

    public async Task<IEnumerable<PackageCategory>> GetAllAsync()
    {
        return await _context.PackageCategories
            .Include(c => c.Packages)
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
    }

    public async Task<PackageCategory> CreateAsync(PackageCategory category)
    {
        _context.PackageCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<PackageCategory> UpdateAsync(PackageCategory category)
    {
        _context.PackageCategories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(int categoryId)
    {
        var category = await _context.PackageCategories.FindAsync(categoryId);
        if (category == null) return false;

        _context.PackageCategories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int categoryId)
    {
        return await _context.PackageCategories.AnyAsync(c => c.CategoryID == categoryId);
    }

    public async Task<IEnumerable<PackageCategory>> GetActiveCategoriesAsync()
    {
        return await _context.PackageCategories
            .Include(c => c.Packages)
            .Where(c => c.CategoryID > 0)
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
    }

    public async Task<int> GetPackageCountByCategoryIdAsync(int categoryId)
    {
        return await _context.Packages.CountAsync(p => p.CategoryID == categoryId);
    }
}
