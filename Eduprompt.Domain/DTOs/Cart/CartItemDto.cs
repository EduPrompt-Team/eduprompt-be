namespace Eduprompt.Domain.DTOs.Cart;

public class CartItemDto
{
    public int CartDetailId { get; set; }
    public int CartId { get; set; }
    public int TemplateId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? SubTotal { get; set; }
    public DateTime? AddedDate { get; set; }
    public string? Status { get; set; }
    
    // Template info
    public string? TemplateName { get; set; }
    public string? TemplateDescription { get; set; }
    public string? PreviewUrl { get; set; }
} 
