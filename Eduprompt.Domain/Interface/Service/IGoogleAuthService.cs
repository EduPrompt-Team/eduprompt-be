using Eduprompt.Domain.DTOs.Auth;

namespace Eduprompt.Domain.Interface.Service;

/// <summary>
/// Interface for Google Authentication Service
/// </summary>
public interface IGoogleAuthService
{
    /// <summary>
    /// Authenticate user with Google ID token
    /// </summary>
    /// <param name="request">Google login request</param>
    /// <returns>Token response with access and refresh tokens</returns>
    Task<TokenResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request);

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New token response</returns>
    Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <returns>True if successful</returns>
    Task<bool> RevokeTokenAsync(string refreshToken);
}
