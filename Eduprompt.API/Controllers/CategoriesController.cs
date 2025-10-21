using Eduprompt.Domain.DTOs.Category;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Category management for prompt templates (Admin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "04. Categories")]
[Produces("application/json")]
[Authorize(Roles = "Admin")] // Only Admin can manage categories
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
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
        var categories = await _categoryService.GetAllAsync();
        var categoryDtos = categories.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            ParentCategoryId = c.ParentCategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            ImageUrl = null, // Not available in current model
            NumberOfTemplates = c.NumberOfTemplates,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate,
            Status = c.Status,
            ParentCategoryName = c.ParentCategoryName,
            SubCategories = c.SubCategories?.Select(sc => new CategoryDto
            {
                CategoryId = sc.CategoryId,
                ParentCategoryId = sc.ParentCategoryId,
                CategoryName = sc.CategoryName,
                Description = sc.Description,
                ImageUrl = null, // Not available in current model
                NumberOfTemplates = sc.NumberOfTemplates,
                Status = sc.Status
            }).ToList()
        });

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Get root categories (categories without parent)
    /// </summary>
    [HttpGet("root")]
    public async Task<IActionResult> GetRootCategories()
    {
        var categories = await _categoryService.GetRootCategoriesAsync();
        var categoryDtos = categories.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            ParentCategoryId = c.ParentCategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            ImageUrl = null,
            NumberOfTemplates = c.NumberOfTemplates,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate,
            Status = c.Status,
            ParentCategoryName = c.ParentCategoryName,
            SubCategories = c.SubCategories?.Select(sc => new CategoryDto
            {
                CategoryId = sc.CategoryId,
                ParentCategoryId = sc.ParentCategoryId,
                CategoryName = sc.CategoryName,
                Description = sc.Description,
                ImageUrl = null,
                NumberOfTemplates = sc.NumberOfTemplates,
                Status = sc.Status
            }).ToList()
        });

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        
        if (category == null)
            return NotFound(new { message = $"Category with ID {id} not found" });

        var categoryDto = new CategoryDto
        {
            CategoryId = category.CategoryId,
            ParentCategoryId = category.ParentCategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            ImageUrl = null,
            NumberOfTemplates = category.NumberOfTemplates,
            CreatedDate = category.CreatedDate,
            UpdatedDate = category.UpdatedDate,
            Status = category.Status,
            ParentCategoryName = category.ParentCategoryName,
            SubCategories = category.SubCategories?.Select(sc => new CategoryDto
            {
                CategoryId = sc.CategoryId,
                ParentCategoryId = sc.ParentCategoryId,
                CategoryName = sc.CategoryName,
                Description = sc.Description,
                ImageUrl = null,
                NumberOfTemplates = sc.NumberOfTemplates,
                Status = sc.Status
            }).ToList()
        };

        return Ok(categoryDto);
    }

    /// <summary>
    /// Get subcategories of a parent category
    /// </summary>
    [HttpGet("{parentId}/subcategories")]
    public async Task<IActionResult> GetSubCategories(int parentId)
    {
        var categories = await _categoryService.GetSubCategoriesAsync(parentId);
        var categoryDtos = categories.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            ParentCategoryId = c.ParentCategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            ImageUrl = null,
            NumberOfTemplates = c.NumberOfTemplates,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate,
            Status = c.Status,
            ParentCategoryName = c.ParentCategoryName
        });

        return Ok(categoryDtos);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto categoryDto)
    {
        try
        {
            var createServiceDto = new CategoryCreateServiceDto
            {
                ParentCategoryId = categoryDto.ParentCategoryId,
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                Status = categoryDto.Status
            };

            var category = await _categoryService.CreateAsync(createServiceDto);
            
            var resultDto = new CategoryDto
            {
                CategoryId = category.CategoryId,
                ParentCategoryId = category.ParentCategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ImageUrl = null,
                NumberOfTemplates = category.NumberOfTemplates,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate,
                Status = category.Status,
                ParentCategoryName = category.ParentCategoryName
            };

            return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, resultDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto categoryDto)
    {
        try
        {
            var updateServiceDto = new CategoryUpdateServiceDto
            {
                ParentCategoryId = categoryDto.ParentCategoryId,
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                Status = categoryDto.Status
            };

            var category = await _categoryService.UpdateAsync(id, updateServiceDto);
            
            var resultDto = new CategoryDto
            {
                CategoryId = category.CategoryId,
                ParentCategoryId = category.ParentCategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ImageUrl = null,
                NumberOfTemplates = category.NumberOfTemplates,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate,
                Status = category.Status,
                ParentCategoryName = category.ParentCategoryName
            };

            return Ok(resultDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Category with ID {id} not found" });

        return NoContent();
    }
} 
