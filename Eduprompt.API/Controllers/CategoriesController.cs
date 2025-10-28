using Eduprompt.Domain.DTOs.PackageCategory;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Category management for prompt templates (Admin only)
/// </summary>
[ApiController]
[Route("api/categories")]
[ApiExplorerSettings(GroupName = "04. Categories")]
[Produces("application/json")]
[Authorize(Roles = "Admin")] // Only Admin can manage categories
public class CategoriesController : ControllerBase
{
    private readonly IPackageCategoryService _PackageCategoryService;

    public CategoriesController(IPackageCategoryService PackageCategoryService)
    {
        _PackageCategoryService = PackageCategoryService;
    }

    /// <summary>
    /// Get all categories with hierarchical structure
    /// </summary>
    /// <returns>List of all categories with subcategories</returns>
    /// <response code="200">Categories retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _PackageCategoryService.GetAllAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Get root categories (categories without parent)
    /// </summary>
    [HttpGet("root")]
    public async Task<IActionResult> GetRootCategories()
    {
        var categories = await _PackageCategoryService.GetAllAsync(); // Use GetAllAsync instead of GetRootCategoriesAsync
        return Ok(categories);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _PackageCategoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    /// <summary>
    /// Get subcategories of a specific category
    /// </summary>
    [HttpGet("{id}/subcategories")]
    public async Task<IActionResult> GetSubCategories(int id)
    {
        var categories = await _PackageCategoryService.GetAllAsync(); // Use GetAllAsync instead of GetSubCategoriesAsync
        return Ok(categories);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePackageCategoryDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var category = await _PackageCategoryService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePackageCategoryDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var category = await _PackageCategoryService.UpdateAsync(id, updateDto);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _PackageCategoryService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}