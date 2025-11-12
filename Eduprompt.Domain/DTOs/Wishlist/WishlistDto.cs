namespace Eduprompt.Domain.DTOs.Wishlist;

public class WishlistDto
{
    public int WishlistId { get; set; }
    public int UserId { get; set; }
    public int? PackageId { get; set; }  // Nullable for backward compatibility
    public int? StorageId { get; set; }  // ID of StorageTemplate
    public DateTime AddedAt { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties - User
    public string? UserName { get; set; }
    
    // Navigation properties - Package (legacy, for backward compatibility)
    public string? PackageName { get; set; }
    public string? PackageDescription { get; set; }
    public decimal? PackagePrice { get; set; }
    
    // Navigation properties - StorageTemplate (prompt template)
    public string? TemplateName { get; set; }
    public string? TemplateContent { get; set; }
    public string? Grade { get; set; }
    public string? Subject { get; set; }
    public string? Chapter { get; set; }
    public bool? IsPublic { get; set; }
    public DateTime? TemplateCreatedAt { get; set; }
} 
