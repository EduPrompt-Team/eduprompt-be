using Eduprompt.Domain.DTOs.Role;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Role management for user permissions (Admin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Only Admin can manage roles
[ApiExplorerSettings(GroupName = "03. Roles (Admin)")]
[Produces("application/json")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Get all roles in the system
    /// </summary>
    /// <returns>List of all roles</returns>
    /// <response code="200">Roles retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Get role details by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <returns>Role details</returns>
    /// <response code="200">Role found</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    /// <response code="404">Role not found</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = await _roleService.GetByIdAsync(id);
        
        if (role == null)
            return NotFound(new { message = $"Role with ID {id} not found" });

        return Ok(role);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleCreateUpdateDto roleDto)
    {
        var role = await _roleService.CreateAsync(roleDto);
        return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, role);
    }

    /// <summary>
    /// Update role information
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] RoleCreateUpdateDto roleDto)
    {
        try
        {
            var role = await _roleService.UpdateAsync(id, roleDto);
            return Ok(role);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _roleService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Role with ID {id} not found" });

        return NoContent();
    }
} 
