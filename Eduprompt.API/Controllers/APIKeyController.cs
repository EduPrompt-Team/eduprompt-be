using Eduprompt.Domain.DTOs.APIKey;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "15. APIKey")]
[Produces("application/json")]
public class APIKeyController : ControllerBase
{
    private readonly IAPIKeyService _service;

    public APIKeyController(IAPIKeyService service)
    {
        _service = service;
    }

    [HttpGet("package/{packageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByPackage(int packageId)
    {
        return Ok(await _service.GetByPackageIdAsync(packageId));
    }

    [HttpGet("active/package/{packageId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveByPackage(int packageId)
    {
        return Ok(await _service.GetActiveKeysByPackageIdAsync(packageId));
    }

    [HttpGet("provider/{provider}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveByProvider(string provider)
    {
        var key = await _service.GetActiveKeyByProviderAsync(provider);
        if (key == null) return NotFound();
        return Ok(key);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAPIKeyDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{apiKeyId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int apiKeyId, [FromBody] CreateAPIKeyDto dto)
    {
        var updated = await _service.UpdateAsync(apiKeyId, dto);
        return Ok(updated);
    }

    [HttpDelete("{apiKeyId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int apiKeyId)
    {
        var ok = await _service.DeleteAsync(apiKeyId);
        return ok ? Ok() : NotFound();
    }
}


