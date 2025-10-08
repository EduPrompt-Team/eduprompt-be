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

    public async Task<RoleDto> CreateAsync(RoleCreateUpdateDto roleDto)
    {
        var role = _mapper.Map<Role>(roleDto);
        var createdRole = await _roleRepository.CreateAsync(role);
        return _mapper.Map<RoleDto>(createdRole);
    }

    public async Task<RoleDto> UpdateAsync(int id, RoleCreateUpdateDto roleDto)
    {
        var existingRole = await _roleRepository.GetByIdAsync(id);
        if (existingRole == null)
        {
            throw new KeyNotFoundException($"Role with ID {id} not found");
        }

        _mapper.Map(roleDto, existingRole);
        
        var updatedRole = await _roleRepository.UpdateAsync(existingRole);
        return _mapper.Map<RoleDto>(updatedRole);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _roleRepository.DeleteAsync(id);
    }
} 
