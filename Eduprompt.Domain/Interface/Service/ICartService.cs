namespace Eduprompt.Domain.Interface.Service;

public interface ICartService
{
    Task<CartServiceDto> GetUserCartAsync(int userId);
    Task<CartServiceDto> AddItemAsync(int userId, AddCartItemServiceDto itemDto);
    Task<CartServiceDto> UpdateItemAsync(int userId, int cartDetailId, UpdateCartItemServiceDto itemDto);
    Task<bool> RemoveItemAsync(int userId, int cartDetailId);
    Task<bool> ClearCartAsync(int userId);
}

public class CartServiceDto
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public int? TotalItem { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    public decimal TotalPrice { get; set; }
    public List<CartItemServiceDto>? Items { get; set; }
}

public class CartItemServiceDto
{
    public int CartDetailId { get; set; }
    public int CartId { get; set; }
    public int TemplateId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? SubTotal { get; set; }
    public DateTime? AddedDate { get; set; }
    public string? Status { get; set; }
    public string? TemplateName { get; set; }
    public string? TemplateDescription { get; set; }
    public string? PreviewUrl { get; set; }
}

public class AddCartItemServiceDto
{
    public int TemplateId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartItemServiceDto
{
    public int Quantity { get; set; }
} 