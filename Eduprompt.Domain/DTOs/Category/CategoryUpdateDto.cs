using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Category;

public class CategoryUpdateDto
{
    public int? ParentCategoryId { get; set; }
    
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string? CategoryName { get; set; }
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    
    [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
    public string? ImageUrl { get; set; } // Category image/icon
    
    public string? Status { get; set; }
} 
