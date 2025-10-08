using Eduprompt.Domain.DTOs.PackageCategory;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class PackageCategoryService : IPackageCategoryService
{
    private readonly IPackageCategoryRepository _packageCategoryRepository;

    public PackageCategoryService(IPackageCategoryRepository packageCategoryRepository)
    {
        _packageCategoryRepository = packageCategoryRepository;
    }

    public async Task<PackageCategoryDto?> GetByIdAsync(int categoryId)
    {
        var category = await _packageCategoryRepository.GetByIdAsync(categoryId);
        if (category == null) return null;

        return new PackageCategoryDto
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            Description = category.Description,
            CreatedDate = category.CreatedDate,
            UpdatedDate = category.UpdatedDate,
            Status = category.Status,
            PackageCount = category.Packages?.Count ?? 0
        };
    }

    public async Task<IEnumerable<PackageCategoryDto>> GetAllAsync()
    {
        var categories = await _packageCategoryRepository.GetAllAsync();
        return categories.Select(c => new PackageCategoryDto
        {
            CategoryID = c.CategoryID,
            CategoryName = c.CategoryName,
            Description = c.Description,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate,
            Status = c.Status,
            PackageCount = c.Packages?.Count ?? 0
        });
    }

    public async Task<PackageCategoryDto> CreateAsync(CreatePackageCategoryDto createDto)
    {
        var category = new Eduprompt.Domain.Entities.PackageCategory
        {
            CategoryName = createDto.CategoryName,
            Description = createDto.Description,
            Status = createDto.Status ?? "Active",
            CreatedDate = DateTime.UtcNow
        };

        var createdCategory = await _packageCategoryRepository.CreateAsync(category);
        return new PackageCategoryDto
        {
            CategoryID = createdCategory.CategoryID,
            CategoryName = createdCategory.CategoryName,
            Description = createdCategory.Description,
            CreatedDate = createdCategory.CreatedDate,
            UpdatedDate = createdCategory.UpdatedDate,
            Status = createdCategory.Status,
            PackageCount = 0
        };
    }

    public async Task<PackageCategoryDto> UpdateAsync(int categoryId, CreatePackageCategoryDto updateDto)
    {
        var category = await _packageCategoryRepository.GetByIdAsync(categoryId);
        if (category == null)
            throw new KeyNotFoundException("Category not found");

        category.CategoryName = updateDto.CategoryName;
        category.Description = updateDto.Description;
        category.Status = updateDto.Status ?? category.Status;
        category.UpdatedDate = DateTime.UtcNow;

        var updatedCategory = await _packageCategoryRepository.UpdateAsync(category);
        return new PackageCategoryDto
        {
            CategoryID = updatedCategory.CategoryID,
            CategoryName = updatedCategory.CategoryName,
            Description = updatedCategory.Description,
            CreatedDate = updatedCategory.CreatedDate,
            UpdatedDate = updatedCategory.UpdatedDate,
            Status = updatedCategory.Status,
            PackageCount = updatedCategory.Packages?.Count ?? 0
        };
    }

    public async Task<bool> DeleteAsync(int categoryId)
    {
        return await _packageCategoryRepository.DeleteAsync(categoryId);
    }

    public async Task<IEnumerable<PackageCategoryDto>> GetActiveCategoriesAsync()
    {
        var categories = await _packageCategoryRepository.GetActiveCategoriesAsync();
        return categories.Select(c => new PackageCategoryDto
        {
            CategoryID = c.CategoryID,
            CategoryName = c.CategoryName,
            Description = c.Description,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate,
            Status = c.Status,
            PackageCount = c.Packages?.Count ?? 0
        });
    }

    public async Task<int> GetPackageCountByCategoryIdAsync(int categoryId)
    {
        return await _packageCategoryRepository.GetPackageCountByCategoryIdAsync(categoryId);
    }
}
