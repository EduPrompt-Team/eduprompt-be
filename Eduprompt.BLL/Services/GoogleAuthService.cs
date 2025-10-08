using AutoMapper;
using Eduprompt.Domain.DTOs.Auth;
using Eduprompt.Domain.DTOs.User;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Eduprompt.BLL.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public GoogleAuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IMapper mapper,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<TokenResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request)
    {
        try
        {
            // Verify Google ID token with audience binding
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _configuration["Google:ClientId"] ?? string.Empty }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);
            
            if (payload == null)
            {
                throw new UnauthorizedAccessException("Invalid Google ID token");
            }

            // Get user info from Google
            var googleUserInfo = new GoogleUserInfoDto
            {
                Id = payload.Subject,
                Email = payload.Email,
                Name = payload.Name,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName,
                Picture = payload.Picture,
                VerifiedEmail = payload.EmailVerified
            };

            // Check if user exists by Google ID or email
            var existingUser = await _userRepository.GetByGoogleIdAsync(googleUserInfo.Id) 
                              ?? await _userRepository.GetByEmailAsync(googleUserInfo.Email);

            User user;
            if (existingUser == null)
            {
                // Create new user
                user = await CreateGoogleUserAsync(googleUserInfo);
            }
            else
            {
                // Update existing user with Google ID if not already set
                if (string.IsNullOrEmpty(existingUser.GoogleId))
                {
                    existingUser.GoogleId = googleUserInfo.Id;
                    existingUser.ProfileUrl = googleUserInfo.Picture;
                    user = await _userRepository.UpdateAsync(existingUser);
                }
                else
                {
                    user = existingUser;
                }
            }

            // Generate tokens
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            
            // Save refresh token hash to user for security
            user.RefreshToken = HashToken(refreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // 7 days expiry
            await _userRepository.UpdateAsync(user);

            var response = new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = Convert.ToInt32(_configuration.GetSection("Jwt")["ExpiresInSeconds"] ?? "3600"),
                User = _mapper.Map<UserDto>(user)
            };

            return response;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException($"Google authentication failed: {ex.Message}");
        }
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(HashToken(request.RefreshToken));
        
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Generate new tokens
        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        
        // Update refresh token
        user.RefreshToken = HashToken(newRefreshToken);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);

        return new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            ExpiresIn = 3600,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(HashToken(refreshToken));
        
        if (user == null)
        {
            return false;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userRepository.UpdateAsync(user);
        
        return true;
    }

    private async Task<User> CreateGoogleUserAsync(GoogleUserInfoDto googleUserInfo)
    {
        // Get default role (User)
        var defaultRole = await _roleRepository.GetByNameAsync("User");
        
        var user = new User
        {
            GoogleId = googleUserInfo.Id,
            FullName = googleUserInfo.Name,
            Email = googleUserInfo.Email,
            ProfileUrl = googleUserInfo.Picture,
            RoleId = defaultRole?.RoleId,
            Status = "Active",
            CreatedDate = DateTime.UtcNow
        };

        return await _userRepository.CreateAsync(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        if (user.Role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role.RoleName));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(Convert.ToDouble(_configuration.GetSection("Jwt")["ExpiresInSeconds"] ?? "3600")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
