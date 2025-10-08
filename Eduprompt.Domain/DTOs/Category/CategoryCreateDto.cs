using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Category;

public class CategoryCreateDto
{
    public int? ParentCategoryId { get; set; }
    
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string CategoryName { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    
    [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
    public string? ImageUrl { get; set; } // Category image/icon
    
    public string? Status { get; set; } = "Active";
} 
