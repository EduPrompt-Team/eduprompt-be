using Eduprompt.Domain.DTOs.Package;

namespace Eduprompt.Domain.Interface.Service;

public interface IPackageService
{
    Task<PackageDto?> GetByIdAsync(int packageId);
    Task<IEnumerable<PackageDto>> GetAllAsync();
    Task<IEnumerable<PackageDto>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<PackageDto>> GetActivePackagesAsync();
    Task<PackageDto> CreateAsync(CreatePackageDto createPackageDto);
    Task<PackageDto> UpdateAsync(int packageId, UpdatePackageDto updatePackageDto);
    Task<bool> DeleteAsync(int packageId);
    Task<IEnumerable<PackageDto>> SearchAsync(string searchTerm);
    Task<IEnumerable<PackageDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
}
