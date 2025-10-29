using Eduprompt.Domain.DTOs.PackageDetail;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/package-details")]
[ApiExplorerSettings(GroupName = "19. PackageDetail")]
[Produces("application/json")]
public class PackageDetailController : ControllerBase
{
    private readonly IPackageDetailService _service;

    public PackageDetailController(IPackageDetailService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get package details by package ID (Public)
    /// </summary>
    /// <param name="PackageId">Package ID</param>
    /// <returns>List of details for the package</returns>
    /// <response code="200">Package details retrieved successfully</response>
    [HttpGet("package/{PackageId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByPackage(int PackageId)
    {
        return Ok(await _service.GetByPackageIdAsync(PackageId));
    }

    /// <summary>
    /// Create new package detail (Admin only)
    /// </summary>
    /// <param name="dto">Package detail creation details</param>
    /// <returns>Created package detail</returns>
    /// <response code="201">Package detail created successfully</response>
    /// <response code="400">Invalid package detail data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePackageDetailDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{detailId}")]
    [Authorize]
    public async Task<IActionResult> Update(int detailId, [FromBody] CreatePackageDetailDto dto)
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


