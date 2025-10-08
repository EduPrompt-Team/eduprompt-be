using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Role;

public class RoleCreateUpdateDto
{
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
    public string RoleName { get; set; } = string.Empty;
    
    public string? Status { get; set; }
} 
