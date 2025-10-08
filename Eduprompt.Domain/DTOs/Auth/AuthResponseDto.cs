namespace Eduprompt.Domain.DTOs.Auth;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ProfileUrl { get; set; }
    public string? RoleName { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
} 
