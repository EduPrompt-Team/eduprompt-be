using Eduprompt.Domain.DTOs.Auth;

namespace Eduprompt.Domain.Interface.Service;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
} 