using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.User;

public class UserUpdateDto
{
    public int? RoleId { get; set; }
    
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string? FullName { get; set; }
    
    [Phone(ErrorMessage = "Invalid phone number")]
    public string? Phone { get; set; }
    
    public string? ProfileUrl { get; set; }
    
    public string? Status { get; set; }
} 
