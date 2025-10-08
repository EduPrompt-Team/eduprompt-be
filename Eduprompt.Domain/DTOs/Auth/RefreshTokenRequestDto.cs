namespace Eduprompt.Domain.DTOs.Auth;

/// <summary>
/// DTO for refresh token request
/// </summary>
public class RefreshTokenRequestDto
{
    /// <summary>
    /// Refresh token to exchange for new access token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
