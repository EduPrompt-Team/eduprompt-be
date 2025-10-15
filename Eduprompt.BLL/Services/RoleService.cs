using AutoMapper;
using Eduprompt.Domain.DTOs.Role;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return role == null ? null : _mapper.Map<RoleDto>(role);
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<RoleDto> CreateAsync(RoleCreateUpdateDto createDto)
    {
        var role = new Role
        {
            RoleName = createDto.RoleName,
            Status = createDto.Status ?? "Active"
        };

        var createdRole = await _roleRepository.CreateAsync(role);
        return _mapper.Map<RoleDto>(createdRole);
    }

    public async Task<RoleDto> UpdateAsync(int roleId, RoleCreateUpdateDto updateDto)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null) throw new KeyNotFoundException("Role not found");

        role.RoleName = updateDto.RoleName;
        role.Status = updateDto.Status ?? role.Status;

        var updatedRole = await _roleRepository.UpdateAsync(role);
        return _mapper.Map<RoleDto>(updatedRole);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _roleRepository.DeleteAsync(id);
    }


        public Task<object?> UpdateAsync(int id, object updateDto)
    {
        return Task.FromResult<object?>(null);
    }
} 






