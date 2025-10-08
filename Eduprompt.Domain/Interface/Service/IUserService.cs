using Eduprompt.Domain.DTOs.User;

namespace Eduprompt.Domain.Interface.Service;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(UserCreateDto userDto);
    Task<UserDto> UpdateAsync(int id, UserUpdateDto userDto);
    Task<bool> DeleteAsync(int id);
} 