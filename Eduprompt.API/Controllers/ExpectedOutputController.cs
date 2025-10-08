using Eduprompt.Domain.DTOs.ExpectedOutput;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "19. ExpectedOutput")]
[Produces("application/json")]
public class ExpectedOutputController : ControllerBase
{
    private readonly IExpectedOutputService _service;

    public ExpectedOutputController(IExpectedOutputService service)
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
    public async Task<IActionResult> Create([FromBody] CreateExpectedOutputDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{outputId}")]
    [Authorize]
    public async Task<IActionResult> Update(int outputId, [FromBody] CreateExpectedOutputDto dto)
    {
        var updated = await _service.UpdateAsync(outputId, dto);
        return Ok(updated);
    }

    [HttpDelete("{outputId}")]
    [Authorize]
    public async Task<IActionResult> Delete(int outputId)
    {
        var ok = await _service.DeleteAsync(outputId);
        return ok ? Ok() : NotFound();
    }
}


