using Eduprompt.Domain.DTOs.StorageTemplate;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Personal storage library for purchased packages
/// </summary>
[ApiController]
[Route("api/storage-templates")]
[Authorize]
[ApiExplorerSettings(GroupName = "09. Storage Templates")]
[Produces("application/json")]
public class StorageTemplatesController : ControllerBase
{
    private readonly IStorageTemplateService _storageService;

    public StorageTemplatesController(IStorageTemplateService storageService)
    {
        _storageService = storageService;
    }

    [HttpGet("my-storage")]
    public async Task<IActionResult> GetMyStorage()
    {
        var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var storage = await _storageService.GetUserStorageAsync(UserId);
        return Ok(storage);
    }

    [HttpPost]
    public async Task<IActionResult> AddToStorage([FromBody] StorageTemplateCreateDto storageDto)
    {
        var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var created = await _storageService.AddToStorageAsync(UserId, new StorageTemplateCreateServiceDto
        {
            TemplateId = storageDto.PackageId,
            TemplateName = storageDto.TemplateName,
            TemplateContent = storageDto.TemplateContent,
            Grade = storageDto.Grade,
            Subject = storageDto.Subject,
            Chapter = storageDto.Chapter,
            IsPublic = storageDto.IsPublic
        });
        return CreatedAtAction(nameof(GetMyStorage), created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromStorage(int id)
    {
        var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _storageService.RemoveFromStorageAsync(id, UserId);
        if (!result)
            return NotFound(new { message = $"Storage item with ID {id} not found" });
        return NoContent();
    }

    [HttpGet("check/{PackageId}")]
    public async Task<IActionResult> CheckStorage(int PackageId)
    {
        var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isInStorage = await _storageService.IsInStorageAsync(UserId, PackageId);
        return Ok(new { PackageId, isInStorage });
    }

    /// <summary>
    /// Get public storage templates (discoverable)
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublic([FromQuery] int? packageId, [FromQuery] string? grade, [FromQuery] string? subject, [FromQuery] string? chapter)
    {
        var list = await _storageService.GetPublicAsync(packageId, grade, subject, chapter);
        return Ok(list);
    }

    /// <summary>
    /// Update a storage template (owner or admin)
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] StorageTemplateUpdateDto request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = User.IsInRole("Admin");
        var dto = new StorageTemplateUpdateServiceDto
        {
            TemplateName = request.TemplateName,
            TemplateContent = request.TemplateContent,
            Grade = request.Grade,
            Subject = request.Subject,
            Chapter = request.Chapter,
            IsPublic = request.IsPublic
        };

        var updated = await _storageService.UpdateAsync(id, userId, dto, isAdmin);
        if (updated == null) return Forbid();
        return Ok(updated);
    }

    /// <summary>
    /// Publish a storage template (Admin only)
    /// </summary>
    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = User.IsInRole("Admin");

        // Allow owners to publish their own templates; admins can publish any template
        if (!isAdmin)
        {
            var myStorage = await _storageService.GetUserStorageAsync(userId);
            var ownsTemplate = myStorage.Any(t => t.StorageId == id);
            if (!ownsTemplate)
            {
                return StatusCode(403, new { message = "You can only publish your own templates" });
            }
        }

        var updated = await _storageService.PublishAsync(id, true, userId, isAdmin);
        if (updated == null)
            return BadRequest(new { message = "Publish failed (missing content, not found, or not owner)" });

        return Ok(new { message = "Published", template = updated });
    }

    /// <summary>
    /// Unpublish a storage template (Admin or Owner)
    /// </summary>
    [HttpPost("{id}/unpublish")]
    public async Task<IActionResult> Unpublish(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = User.IsInRole("Admin");
        var updated = await _storageService.PublishAsync(id, false, userId, isAdmin);
        if (updated == null)
            return Forbid();
        return Ok(new { message = "Unpublished", template = updated });
    }
} 
