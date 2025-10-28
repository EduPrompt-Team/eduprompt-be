using Eduprompt.Domain.DTOs.PromptInstanceDetail;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/prompt-instances/{instanceId}/details")]
[ApiExplorerSettings(GroupName = "18. PromptInstanceDetail")]
[Produces("application/json")]
public class PromptInstanceDetailController : ControllerBase
{
    private readonly IPromptInstanceDetailService _service;

    public PromptInstanceDetailController(IPromptInstanceDetailService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get prompt instance details by instance ID (Public)
    /// </summary>
    /// <param name="instanceId">Prompt instance ID</param>
    /// <returns>List of details for the prompt instance</returns>
    /// <response code="200">Prompt instance details retrieved successfully</response>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetByInstance(int instanceId)
    {
        return Ok(await _service.GetByInstanceIdAsync(instanceId));
    }

    /// <summary>
    /// Create new prompt instance detail
    /// </summary>
    /// <param name="dto">Prompt instance detail creation details</param>
    /// <returns>Created prompt instance detail</returns>
    /// <response code="201">Prompt instance detail created successfully</response>
    /// <response code="400">Invalid prompt instance detail data</response>
    /// <response code="401">User not authenticated</response>
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


