using AutoMapper;
using Eduprompt.Domain.DTOs.Auth;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.DTOs.User;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Eduprompt.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IWalletService _walletService;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IMapper mapper,
        IConfiguration configuration,
        IWalletService walletService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _configuration = configuration;
        _walletService = walletService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check if user already exists
        if (await _userRepository.ExistsAsync(request.Email))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Get default role (User)
        var defaultRole = await _roleRepository.GetByNameAsync("User");
        
        // Create new user
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Password = HashPassword(request.Password),
            RoleId = defaultRole?.RoleId,
            Status = "Active"
        };

        var createdUser = await _userRepository.CreateAsync(user);
        
        // Auto-create wallet for new user
        try
        {
            // Check if wallet already exists
            var existingWallet = await _walletService.GetByUserIdAsync(createdUser.UserId);
            if (existingWallet == null)
            {
                await _walletService.CreateAsync(new Domain.DTOs.Wallet.CreateWalletDto
                {
                    UserId = createdUser.UserId,
                    Currency = "VND",
                    Status = "Active"
                });
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail registration if wallet creation fails
            // User can create wallet manually later
            Console.WriteLine($"Warning: Failed to auto-create wallet for user {createdUser.UserId}: {ex.Message}");
        }
        
        // Generate token
        var response = _mapper.Map<AuthResponseDto>(createdUser);
        response.Token = GenerateJwtToken(createdUser);

        return response;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password
        if (!VerifyPassword(request.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if user is active
        if (user.Status != "Active")
        {
            throw new UnauthorizedAccessException("User account is not active");
        }

        // Generate access + refresh token (align with Google flow)
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = HashToken(refreshToken);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = Convert.ToInt32(_configuration.GetSection("Jwt")["ExpiresInSeconds"] ?? "3600"),
            User = _mapper.Map<UserDto>(user)
        };
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hashedPassword;
    }

    private async Task<string> GenerateJwtTokenAsync(User user)
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

        // Load role if not already loaded
        if (user.Role == null && user.RoleId.HasValue)
        {
            var role = await _roleRepository.GetByIdAsync(user.RoleId.Value);
            if (role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
            }
        }
        else if (user.Role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role.RoleName));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(Convert.ToDouble(jwtSettings["ExpiresInSeconds"] ?? "3600")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string GenerateJwtToken(User user)
    {
        // Backward compatibility wrapper
        return GenerateJwtTokenAsync(user).GetAwaiter().GetResult();
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _userRepository.DeleteAsync(id);
    }

    public Task<object?> UpdateAsync(int id, object updateDto)
    {
        // Implementation will be added based on specific service needs
        return Task.FromResult<object?>(null);
    }
} 







