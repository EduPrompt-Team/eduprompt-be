using Eduprompt.Domain.DTOs.User;

namespace Eduprompt.Domain.DTOs.Auth;

/// <summary>
/// DTO for token response with access and refresh tokens
/// </summary>
public class TokenResponseDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Refresh token for getting new access tokens
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Token type (Bearer)
    /// </summary>
    public string TokenType { get; set; } = "Bearer";
    
    /// <summary>
    /// Access token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }
    
    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; set; } = new();
}
