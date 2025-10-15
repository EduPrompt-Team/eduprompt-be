using Eduprompt.Domain.DTOs.PackageCategory;
using Eduprompt.Domain.Entities;
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
        return category != null ? MapToDto(category) : null;
    }

    public async Task<IEnumerable<PackageCategoryDto>> GetAllAsync()
    {
        var categories = await _packageCategoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<PackageCategoryDto> CreateAsync(CreatePackageCategoryDto createDto)
    {
        var category = new PackageCategory
        {
            CategoryName = createDto.CategoryName,
            Description = createDto.Description
        };

        var createdCategory = await _packageCategoryRepository.CreateAsync(category);
        return MapToDto(createdCategory);
    }

    public async Task<PackageCategoryDto> UpdateAsync(int categoryId, CreatePackageCategoryDto updateDto)
    {
        var category = await _packageCategoryRepository.GetByIdAsync(categoryId);
        if (category == null) throw new KeyNotFoundException("Package category not found");

        category.CategoryName = updateDto.CategoryName;
        category.Description = updateDto.Description;

        var updatedCategory = await _packageCategoryRepository.UpdateAsync(category);
        return MapToDto(updatedCategory);
    }

    public async Task<bool> DeleteAsync(int categoryId)
    {
        return await _packageCategoryRepository.DeleteAsync(categoryId);
    }

    public async Task<IEnumerable<PackageCategoryDto>> GetActiveCategoriesAsync()
    {
        var categories = await _packageCategoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<int> GetPackageCountByCategoryIdAsync(int categoryId)
    {
        return await _packageCategoryRepository.GetPackageCountByCategoryIdAsync(categoryId);
    }

    private static PackageCategoryDto MapToDto(PackageCategory category)
    {
        return new PackageCategoryDto
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            Description = category.Description,
            PackageCount = category.Packages?.Count ?? 0
        };
    }
}