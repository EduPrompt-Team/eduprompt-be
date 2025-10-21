using Eduprompt.Domain.DTOs.TemplateArchitecture;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "17. TemplateArchitecture")]
[Produces("application/json")]
public class TemplateArchitectureController : ControllerBase
{
    private readonly ITemplateArchitectureService _service;

    public TemplateArchitectureController(ITemplateArchitectureService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get template architecture by instance ID (Public)
    /// </summary>
    /// <param name="instanceId">Prompt instance ID</param>
    /// <returns>Template architecture for the instance</returns>
    /// <response code="200">Template architecture retrieved successfully</response>
    [HttpGet("instance/{instanceId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByInstance(int instanceId)
    {
        return Ok(await _service.GetByPromptInstanceIdAsync(instanceId));
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
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateTemplateArchitectureDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{architectureId}")]
    [Authorize]
    public async Task<IActionResult> Update(int architectureId, [FromBody] CreateTemplateArchitectureDto dto)
    {
        var updated = await _service.UpdateAsync(architectureId, dto);
        return Ok(updated);
    }

    [HttpDelete("{architectureId}")]
    [Authorize]
    public async Task<IActionResult> Delete(int architectureId)
    {
        var ok = await _service.DeleteAsync(architectureId);
        return ok ? Ok() : NotFound();
    }
}


