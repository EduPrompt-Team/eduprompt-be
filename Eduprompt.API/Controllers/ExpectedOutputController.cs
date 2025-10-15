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

    /// <summary>
    /// Get expected outputs by instance ID (Public)
    /// </summary>
    /// <param name="instanceId">Prompt instance ID</param>
    /// <returns>List of expected outputs for the instance</returns>
    /// <response code="200">Expected outputs retrieved successfully</response>
    [HttpGet("instance/{instanceId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByInstance(int instanceId)
    {
        return Ok(await _service.GetByInstanceIdAsync(instanceId));
    }

    /// <summary>
    /// Create new expected output
    /// </summary>
    /// <param name="dto">Expected output creation details</param>
    /// <returns>Created expected output</returns>
    /// <response code="201">Expected output created successfully</response>
    /// <response code="400">Invalid expected output data</response>
    /// <response code="401">User not authenticated</response>
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


