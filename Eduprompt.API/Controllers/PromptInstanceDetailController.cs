using Eduprompt.Domain.DTOs.PromptInstanceDetail;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "18. PromptInstanceDetail")]
[Produces("application/json")]
public class PromptInstanceDetailController : ControllerBase
{
    private readonly IPromptInstanceDetailService _service;

    public PromptInstanceDetailController(IPromptInstanceDetailService service)
    {
        _service = service;
    }

    [HttpGet("instance/{instanceId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByInstance(int instanceId)
    {
        return Ok(await _service.GetByInstanceIdAsync(instanceId));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePromptInstanceDetailDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{detailId}")]
    [Authorize]
    public async Task<IActionResult> Update(int detailId, [FromBody] CreatePromptInstanceDetailDto dto)
    {
        var updated = await _service.UpdateAsync(detailId, dto);
        return Ok(updated);
    }

    [HttpDelete("{detailId}")]
    [Authorize]
    public async Task<IActionResult> Delete(int detailId)
    {
        var ok = await _service.DeleteAsync(detailId);
        return ok ? Ok() : NotFound();
    }
}


