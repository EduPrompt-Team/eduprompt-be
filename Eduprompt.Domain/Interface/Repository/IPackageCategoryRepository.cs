using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IPackageCategoryRepository
{
    Task<PackageCategory?> GetByIdAsync(int categoryId);
    Task<IEnumerable<PackageCategory>> GetAllAsync();
    Task<PackageCategory> CreateAsync(PackageCategory category);
    Task<PackageCategory> UpdateAsync(PackageCategory category);
    Task<bool> DeleteAsync(int categoryId);
    Task<bool> ExistsAsync(int categoryId);
    Task<IEnumerable<PackageCategory>> GetActiveCategoriesAsync();
    Task<int> GetPackageCountByCategoryIdAsync(int categoryId);
}
