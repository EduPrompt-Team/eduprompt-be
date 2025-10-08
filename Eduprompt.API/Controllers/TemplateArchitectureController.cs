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

    [HttpGet("instance/{instanceId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByInstance(int instanceId)
    {
        return Ok(await _service.GetByPromptInstanceIdAsync(instanceId));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateTemplateArchitectureDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{architectureId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int architectureId, [FromBody] CreateTemplateArchitectureDto dto)
    {
        var updated = await _service.UpdateAsync(architectureId, dto);
        return Ok(updated);
    }

    [HttpDelete("{architectureId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int architectureId)
    {
        var ok = await _service.DeleteAsync(architectureId);
        return ok ? Ok() : NotFound();
    }
}


