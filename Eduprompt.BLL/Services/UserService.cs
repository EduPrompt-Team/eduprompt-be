using AutoMapper;
using Eduprompt.Domain.DTOs.User;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using System.Security.Cryptography;
using System.Text;

namespace Eduprompt.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> CreateAsync(UserCreateDto userDto)
    {
        // Check if user already exists
        if (await _userRepository.ExistsAsync(userDto.Email))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = _mapper.Map<User>(userDto);
        user.Password = HashPassword(userDto.Password);
        
        var createdUser = await _userRepository.CreateAsync(user);
        return _mapper.Map<UserDto>(createdUser);
    }

    public async Task<UserDto> UpdateAsync(int id, UserUpdateDto userDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        _mapper.Map(userDto, existingUser);
        
        var updatedUser = await _userRepository.UpdateAsync(existingUser);
        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
} 
