using Eduprompt.Domain.DTOs.PackageCategory;

namespace Eduprompt.Domain.Interface.Service;

public interface IPackageCategoryService
{
    Task<PackageCategoryDto?> GetByIdAsync(int categoryId);
    Task<IEnumerable<PackageCategoryDto>> GetAllAsync();
    Task<PackageCategoryDto> CreateAsync(CreatePackageCategoryDto createDto);
    Task<PackageCategoryDto> UpdateAsync(int categoryId, CreatePackageCategoryDto updateDto);
    Task<bool> DeleteAsync(int categoryId);
    Task<IEnumerable<PackageCategoryDto>> GetActiveCategoriesAsync();
    Task<int> GetPackageCountByCategoryIdAsync(int categoryId);
}
