using Eduprompt.Domain.DTOs.Role;

namespace Eduprompt.Domain.Interface.Service;

public interface IRoleService
{
    Task<RoleDto?> GetByIdAsync(int id);
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto> CreateAsync(RoleCreateUpdateDto roleDto);
    Task<RoleDto> UpdateAsync(int id, RoleCreateUpdateDto roleDto);
    Task<bool> DeleteAsync(int id);
} 