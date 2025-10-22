namespace Eduprompt.Domain.DTOs.Auth;

/// <summary>
/// DTO for Google login request
/// </summary>
public class GoogleLoginRequestDto
{
    /// <summary>
    /// Google ID token from client
    /// </summary>
    public string IdToken { get; set; } = string.Empty;
    
    public string? Provider { get; set; } = "Google";
    /// <summary>
    /// Google access token from client
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
}
