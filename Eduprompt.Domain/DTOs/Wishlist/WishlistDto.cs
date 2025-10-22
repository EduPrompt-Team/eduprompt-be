namespace Eduprompt.Domain.DTOs.Wishlist;

public class WishlistDto
{
    public int WishlistId { get; set; }
    public int UserId { get; set; }
    public int PackageId { get; set; }
    public DateTime AddedAt { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public string? UserName { get; set; }
    public string? PackageName { get; set; }
    public string? PackageDescription { get; set; }
    public decimal? PackagePrice { get; set; }
} 
