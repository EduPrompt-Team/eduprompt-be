using Eduprompt.Domain.DTOs.TemplateArchitecture;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/template-architectures")]
[ApiExplorerSettings(GroupName = "21. TemplateArchitecture")]
[Produces("application/json")]
public class TemplateArchitectureController : ControllerBase
{
    private readonly ITemplateArchitectureService _service;

    public TemplateArchitectureController(ITemplateArchitectureService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get template architecture by ID (Public)
    /// </summary>
    /// <param name="id">Template architecture ID</param>
    /// <returns>Template architecture details</returns>
    /// <response code="200">Template architecture retrieved successfully</response>
    /// <response code="404">Template architecture not found</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(new { message = "Template architecture not found" });
        return Ok(result);
    }

    /// <summary>
    /// Get template architecture by instance ID (Public)
    /// </summary>
    /// <param name="InstanceId">Prompt instance ID</param>
    /// <returns>Template architecture for the instance</returns>
    /// <response code="200">Template architecture retrieved successfully</response>
    [HttpGet("instance/{InstanceId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByInstance(int InstanceId)
    {
        return Ok(await _service.GetByPromptInstanceIdAsync(InstanceId));
    }

    /// <summary>
    /// Create new template architecture (Admin only)
    /// </summary>
    /// <param name="dto">Template architecture creation details</param>
    /// <returns>Created template architecture</returns>
    /// <response code="201">Template architecture created successfully</response>
    /// <response code="400">Invalid template architecture data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateTemplateArchitectureDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.ArchitectureId }, created);
    }

    [HttpPut("{architectureId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int architectureId, [FromBody] CreateTemplateArchitectureDto dto)
    {
        var updated = await _service.UpdateAsync(architectureId, dto);
        return Ok(updated);
    }

    [HttpDelete("{architectureId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int architectureId)
    {
        var ok = await _service.DeleteAsync(architectureId);
        return ok ? Ok() : NotFound();
    }

    /// <summary>
    /// Get all template architectures (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }
}


