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
    /// Get all packages
    /// </summary>
    /// <returns>List of all packages</returns>
    /// <response code="200">Packages retrieved successfully</response>
    /// <response code="400">Error retrieving packages</response>
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
    /// Get package by ID
    /// </summary>
    /// <param name="packageId">Package ID</param>
    /// <returns>Package details</returns>
    /// <response code="200">Package found</response>
    /// <response code="400">Error retrieving package</response>
    /// <response code="404">Package not found</response>
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
    /// Get packages by category ID
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>List of packages in the category</returns>
    /// <response code="200">Packages retrieved successfully</response>
    /// <response code="400">Error retrieving packages</response>
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
    /// Get active packages only
    /// </summary>
    /// <returns>List of active packages</returns>
    /// <response code="200">Active packages retrieved successfully</response>
    /// <response code="400">Error retrieving packages</response>
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
    /// Search packages by search term
    /// </summary>
    /// <param name="searchTerm">Search term to filter packages</param>
    /// <returns>List of matching packages</returns>
    /// <response code="200">Search results retrieved successfully</response>
    /// <response code="400">Search term is required or error occurred</response>
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
    /// Get packages within price range
    /// </summary>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <returns>List of packages within price range</returns>
    /// <response code="200">Packages retrieved successfully</response>
    /// <response code="400">Error retrieving packages</response>
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
    /// Create a new package (Admin only)
    /// </summary>
    /// <param name="createPackageDto">Package creation details</param>
    /// <returns>Created package details</returns>
    /// <response code="201">Package created successfully</response>
    /// <response code="400">Invalid package data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
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
    /// Update package by ID (Admin only)
    /// </summary>
    /// <param name="packageId">Package ID to update</param>
    /// <param name="updatePackageDto">Updated package information</param>
    /// <returns>Updated package details</returns>
    /// <response code="200">Package updated successfully</response>
    /// <response code="400">Invalid package data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    /// <response code="404">Package not found</response>
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
    /// Delete package by ID (Admin only)
    /// </summary>
    /// <param name="packageId">Package ID to delete</param>
    /// <returns>Success message</returns>
    /// <response code="200">Package deleted successfully</response>
    /// <response code="400">Error deleting package</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    /// <response code="404">Package not found</response>
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
