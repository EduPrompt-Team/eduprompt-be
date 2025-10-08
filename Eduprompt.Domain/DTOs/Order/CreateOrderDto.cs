using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Order;

public class CreateOrderDto
{
    // Order will be created from current user's cart
    // No need for additional fields, just trigger checkout
    
    public string? Notes { get; set; }
} 
