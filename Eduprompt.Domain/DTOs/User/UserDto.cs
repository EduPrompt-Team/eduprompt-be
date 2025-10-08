namespace Eduprompt.Domain.DTOs.User;

public class UserDto
{
    public int UserId { get; set; }
    public int? RoleId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ProfileUrl { get; set; }
    public string? RoleName { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
} 
