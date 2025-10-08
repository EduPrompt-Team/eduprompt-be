using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IPackageRepository
{
    Task<Package?> GetByIdAsync(int packageId);
    Task<IEnumerable<Package>> GetAllAsync();
    Task<IEnumerable<Package>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Package>> GetActivePackagesAsync();
    Task<Package> CreateAsync(Package package);
    Task<Package> UpdateAsync(Package package);
    Task<bool> DeleteAsync(int packageId);
    Task<bool> ExistsAsync(int packageId);
    Task<IEnumerable<Package>> SearchAsync(string searchTerm);
    Task<IEnumerable<Package>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
}
