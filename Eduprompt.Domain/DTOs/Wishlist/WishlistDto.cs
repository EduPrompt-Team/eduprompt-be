namespace Eduprompt.Domain.DTOs.Wishlist;

public class WishlistDto
{
    public int WishlistId { get; set; }
    public int UserId { get; set; }
    public int TemplateId { get; set; }
    public string? WishlistName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? Status { get; set; }
    
    // Navigation properties
    public string? UserName { get; set; }
    public string? TemplateName { get; set; }
    public string? TemplateDescription { get; set; }
    public decimal? TemplatePrice { get; set; }
    public string? TemplatePreviewUrl { get; set; }
} 
