using Eduprompt.Domain.DTOs.Package;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "13. Package")]
[Produces("application/json")]
public class PackageController : ControllerBase
{
    private readonly IPackageService _packageService;

    public PackageController(IPackageService packageService)
    {
        _packageService = packageService;
    }

    /// <summary>
    /// Lấy tất cả gói
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var packages = await _packageService.GetAllAsync();
            return Ok(packages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy gói theo ID
    /// </summary>
    [HttpGet("{packageId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int packageId)
    {
        try
        {
            var package = await _packageService.GetByIdAsync(packageId);
            if (package == null)
                return NotFound(new { message = "Package not found" });

            return Ok(package);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy gói theo danh mục
    /// </summary>
    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        try
        {
            var packages = await _packageService.GetByCategoryIdAsync(categoryId);
            return Ok(packages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy gói đang hoạt động
    /// </summary>
    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActivePackages()
    {
        try
        {
            var packages = await _packageService.GetActivePackagesAsync();
            return Ok(packages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tìm kiếm gói
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrEmpty(searchTerm))
                return BadRequest(new { message = "Search term is required" });

            var packages = await _packageService.SearchAsync(searchTerm);
            return Ok(packages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy gói theo khoảng giá
    /// </summary>
    [HttpGet("price-range")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        try
        {
            var packages = await _packageService.GetByPriceRangeAsync(minPrice, maxPrice);
            return Ok(packages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo gói mới
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePackageDto createPackageDto)
    {
        try
        {
            var package = await _packageService.CreateAsync(createPackageDto);
            return CreatedAtAction(nameof(GetById), new { packageId = package.PackageID }, package);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật gói
    /// </summary>
    [HttpPut("{packageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int packageId, [FromBody] UpdatePackageDto updatePackageDto)
    {
        try
        {
            var package = await _packageService.UpdateAsync(packageId, updatePackageDto);
            return Ok(package);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa gói
    /// </summary>
    [HttpDelete("{packageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int packageId)
    {
        try
        {
            var result = await _packageService.DeleteAsync(packageId);
            if (!result)
                return NotFound(new { message = "Package not found" });

            return Ok(new { message = "Package deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
