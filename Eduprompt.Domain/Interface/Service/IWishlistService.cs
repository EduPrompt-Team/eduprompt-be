namespace Eduprompt.Domain.Interface.Service;

public interface IWishlistService
{
    Task<IEnumerable<WishlistServiceDto>> GetUserWishlistAsync(int userId);
    Task<WishlistServiceDto> AddToWishlistAsync(int userId, WishlistCreateServiceDto wishlistDto);
    Task<bool> RemoveFromWishlistAsync(int id, int userId);
    Task<bool> IsInWishlistAsync(int userId, int templateId);
}

public class WishlistServiceDto
{
    public int WishlistId { get; set; }
    public int UserId { get; set; }
    public int TemplateId { get; set; }
    public string? WishlistName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? TemplateName { get; set; }
    public string? TemplateDescription { get; set; }
    public decimal? TemplatePrice { get; set; }
    public string? TemplatePreviewUrl { get; set; }
}

public class WishlistCreateServiceDto
{
    public int TemplateId { get; set; }
    public string? WishlistName { get; set; }
} 