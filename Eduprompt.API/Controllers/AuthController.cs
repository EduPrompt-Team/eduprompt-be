using Eduprompt.Domain.DTOs.Auth;
using Eduprompt.Domain.DTOs.User;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Authentication and user management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "01. Authentication")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthController(IAuthService authService, IUserService userService, IGoogleAuthService googleAuthService)
    {
        _authService = authService;
        _userService = userService;
        _googleAuthService = googleAuthService;
    }

    /// <summary>
    /// Register a new user account with email and password
    /// </summary>
    /// <param name="request">Registration details including email, password, and user information</param>
    /// <returns>Registration response with user details and authentication tokens</returns>
    /// <response code="200">Registration successful</response>
    /// <response code="400">Invalid registration data or email already exists</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    /// <param name="request">Login credentials (email and password)</param>
    /// <returns>Authentication response with access token and refresh token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticate user with Google OAuth ID token
    /// </summary>
    /// <param name="request">Google OAuth request containing ID token</param>
    /// <returns>Authentication response with access token and refresh token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid Google ID token</response>
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
    {
        try
        {
            var response = await _googleAuthService.GoogleLoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Refresh access token using valid refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New access token and refresh token</returns>
    /// <response code="200">Token refresh successful</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var response = await _googleAuthService.RefreshTokenAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Revoke refresh token to logout user
    /// </summary>
    /// <param name="request">Refresh token to revoke</param>
    /// <returns>Success message</returns>
    /// <response code="200">Token revoked successfully</response>
    /// <response code="400">Invalid refresh token</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _googleAuthService.RevokeTokenAsync(request.RefreshToken);
        
        if (!result)
            return BadRequest(new { message = "Invalid refresh token" });

        return Ok(new { message = "Token revoked successfully" });
    }

    /// <summary>
    /// Get current authenticated user profile information
    /// </summary>
    /// <returns>Current user profile details</returns>
    /// <response code="200">User profile retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.GetByIdAsync(userId);
        
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }
} 
