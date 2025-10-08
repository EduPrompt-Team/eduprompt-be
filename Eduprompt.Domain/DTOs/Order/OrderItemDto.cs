namespace Eduprompt.Domain.DTOs.Order;

public class OrderItemDto
{
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public int TemplateId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal? SubTotal { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? Status { get; set; }
    
    // Template info
    public string? TemplateName { get; set; }
    public string? TemplateDescription { get; set; }
    public string? PreviewUrl { get; set; }
} 
