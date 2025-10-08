using Eduprompt.Domain.DTOs.PackageCategory;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// 📦 Package Categories - Quản lý danh mục gói sản phẩm
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "14. Package Categories")]
[Produces("application/json")]
[Authorize]
public class PackageCategoryController : ControllerBase
{
    private readonly IPackageCategoryService _packageCategoryService;

    public PackageCategoryController(IPackageCategoryService packageCategoryService)
    {
        _packageCategoryService = packageCategoryService;
    }

    /// <summary>
    /// Lấy danh sách tất cả danh mục
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var categories = await _packageCategoryService.GetAllAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách danh mục đang hoạt động
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var categories = await _packageCategoryService.GetActiveCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết danh mục
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var category = await _packageCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo danh mục mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePackageCategoryDto createDto)
    {
        try
        {
            var category = await _packageCategoryService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = category.CategoryID }, category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật danh mục
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePackageCategoryDto updateDto)
    {
        try
        {
            var category = await _packageCategoryService.UpdateAsync(id, updateDto);
            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa danh mục
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _packageCategoryService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy số lượng gói trong danh mục
    /// </summary>
    [HttpGet("{id}/package-count")]
    public async Task<IActionResult> GetPackageCount(int id)
    {
        try
        {
            var count = await _packageCategoryService.GetPackageCountByCategoryIdAsync(id);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
