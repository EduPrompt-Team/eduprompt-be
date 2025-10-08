namespace Eduprompt.Domain.DTOs.Category;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public int? ParentCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; } // Category image/icon
    public int? NumberOfTemplates { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    
    // Navigation properties
    public string? ParentCategoryName { get; set; }
    public List<CategoryDto>? SubCategories { get; set; }
} 
