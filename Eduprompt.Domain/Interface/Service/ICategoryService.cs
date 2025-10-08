namespace Eduprompt.Domain.Interface.Service;

public interface ICategoryService
{
    Task<CategoryServiceDto?> GetByIdAsync(int id);
    Task<IEnumerable<CategoryServiceDto>> GetAllAsync();
    Task<IEnumerable<CategoryServiceDto>> GetRootCategoriesAsync();
    Task<IEnumerable<CategoryServiceDto>> GetSubCategoriesAsync(int parentId);
    Task<CategoryServiceDto> CreateAsync(CategoryCreateServiceDto categoryDto);
    Task<CategoryServiceDto> UpdateAsync(int id, CategoryUpdateServiceDto categoryDto);
    Task<bool> DeleteAsync(int id);
}

// Service DTOs (used internally between layers)
public class CategoryServiceDto
{
    public int CategoryId { get; set; }
    public int? ParentCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? NumberOfTemplates { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<CategoryServiceDto>? SubCategories { get; set; }
}

public class CategoryCreateServiceDto
{
    public int? ParentCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Status { get; set; }
}

public class CategoryUpdateServiceDto
{
    public int? ParentCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
} 