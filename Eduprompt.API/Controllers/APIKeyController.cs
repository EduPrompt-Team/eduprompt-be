using Eduprompt.Domain.DTOs.Apikey;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/api-keys")]
[ApiExplorerSettings(GroupName = "15. APIKey")]
[Produces("application/json")]
public class APIKeyController : ControllerBase
{
    private readonly IApikeyService _service;

    public APIKeyController(IApikeyService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get API keys by package ID (Admin only)
    /// </summary>
    /// <param name="PackageId">Package ID</param>
    /// <returns>List of API keys for the package</returns>
    /// <response code="200">API keys retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpGet("package/{PackageId}")]
    [Authorize]
    public async Task<IActionResult> GetByPackage(int PackageId)
    {
        return Ok(await _service.GetByPackageIdAsync(PackageId));
    }

    /// <summary>
    /// Get active API keys by package ID (Public)
    /// </summary>
    /// <param name="PackageId">Package ID</param>
    /// <returns>List of active API keys for the package</returns>
    /// <response code="200">Active API keys retrieved successfully</response>
    [HttpGet("active/package/{PackageId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveByPackage(int PackageId)
    {
        return Ok(await _service.GetActiveKeysByPackageIdAsync(PackageId));
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
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateApikeyDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{apiKeyId}")]
    [Authorize]
    public async Task<IActionResult> Update(int apiKeyId, [FromBody] CreateApikeyDto dto)
    {
        var updated = await _service.UpdateAsync(apiKeyId, dto);
        return Ok(updated);
    }

    [HttpDelete("{apiKeyId}")]
    [Authorize]
    public async Task<IActionResult> Delete(int apiKeyId)
    {
        var ok = await _service.DeleteAsync(apiKeyId);
        return ok ? Ok() : NotFound();
    }
}


