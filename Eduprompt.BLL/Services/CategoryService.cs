using Eduprompt.Domain.DTOs.PackageCategory;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly IPackageCategoryService _packageCategoryService;

    public CategoryService(IPackageCategoryService packageCategoryService)
    {
        _packageCategoryService = packageCategoryService;
    }

    public async Task<CategoryServiceDto?> GetByIdAsync(int id)
    {
        var cat = await _packageCategoryService.GetByIdAsync(id);
        if (cat == null) return null;
        return Map(cat);
    }

    public async Task<IEnumerable<CategoryServiceDto>> GetAllAsync()
    {
        var cats = await _packageCategoryService.GetAllAsync();
        return cats.Select(Map);
    }

    public async Task<IEnumerable<CategoryServiceDto>> GetRootCategoriesAsync()
    {
        // Current model has no parent-child; treat all as roots
        var cats = await _packageCategoryService.GetAllAsync();
        return cats.Select(Map);
    }

    public Task<IEnumerable<CategoryServiceDto>> GetSubCategoriesAsync(int parentId)
    {
        // No hierarchy in current entity; return empty
        return Task.FromResult(Enumerable.Empty<CategoryServiceDto>());
    }

    public async Task<CategoryServiceDto> CreateAsync(CategoryCreateServiceDto categoryDto)
    {
        var create = new CreatePackageCategoryDto
        {
            CategoryName = categoryDto.CategoryName,
            Description = categoryDto.Description,
            Status = categoryDto.Status
        };

        var created = await _packageCategoryService.CreateAsync(create);
        return Map(created);
    }

    public async Task<CategoryServiceDto> UpdateAsync(int id, CategoryUpdateServiceDto categoryDto)
    {
        var updated = await _packageCategoryService.UpdateAsync(id, new CreatePackageCategoryDto
        {
            CategoryName = categoryDto.CategoryName ?? string.Empty,
            Description = categoryDto.Description,
            Status = categoryDto.Status
        });
        return Map(updated);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _packageCategoryService.DeleteAsync(id);
    }

    private static CategoryServiceDto Map(PackageCategoryDto cat)
    {
        return new CategoryServiceDto
        {
            CategoryId = cat.CategoryID,
            ParentCategoryId = null, // No parent-child relationship in current model
            CategoryName = cat.CategoryName,
            Description = cat.Description,
            NumberOfTemplates = cat.PackageCount,
            CreatedDate = DateTime.UtcNow, // Default value since not in database
            UpdatedDate = null, // Not available in current model
            Status = "Active", // Default value since not in database
            ParentCategoryName = null, // No parent-child relationship
            SubCategories = null // No subcategories in current model
        };
    }
}


