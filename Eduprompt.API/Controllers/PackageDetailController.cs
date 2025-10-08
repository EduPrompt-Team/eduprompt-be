using Eduprompt.Domain.DTOs.PackageDetail;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "16. PackageDetail")]
[Produces("application/json")]
public class PackageDetailController : ControllerBase
{
    private readonly IPackageDetailService _service;

    public PackageDetailController(IPackageDetailService service)
    {
        _service = service;
    }

    [HttpGet("package/{packageId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByPackage(int packageId)
    {
        return Ok(await _service.GetByPackageIdAsync(packageId));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePackageDetailDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{detailId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int detailId, [FromBody] CreatePackageDetailDto dto)
    {
        var updated = await _service.UpdateAsync(detailId, dto);
        return Ok(updated);
    }

    [HttpDelete("{detailId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int detailId)
    {
        var ok = await _service.DeleteAsync(detailId);
        return ok ? Ok() : NotFound();
    }
}


