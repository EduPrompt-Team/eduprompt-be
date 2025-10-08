using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.User;

public class UserCreateDto
{
    public int? RoleId { get; set; }
    
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
    
    [Phone(ErrorMessage = "Invalid phone number")]
    public string? Phone { get; set; }
    
    public string? ProfileUrl { get; set; }
} 
