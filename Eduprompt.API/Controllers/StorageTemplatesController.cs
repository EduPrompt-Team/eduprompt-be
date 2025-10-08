using Eduprompt.Domain.DTOs.StorageTemplate;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Personal storage library for purchased templates
/// </summary>
[ApiController]
[Route("api/storage-templates")]
[Authorize] // All storage operations require authentication
[ApiExplorerSettings(GroupName = "09. Storage Templates")]
[Produces("application/json")]
public class StorageTemplatesController : ControllerBase
{
    private readonly IStorageTemplateService _storageService;

    public StorageTemplatesController(IStorageTemplateService storageService)
    {
        _storageService = storageService;
    }

    /// <summary>
    /// Get current user's personal storage library
    /// </summary>
    /// <returns>List of templates in user's storage</returns>
    /// <response code="200">Storage retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("my-storage")]
    public async Task<IActionResult> GetMyStorage()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var storage = await _storageService.GetUserStorageAsync(userId);
        
        var storageDtos = storage.Select(s => new StorageTemplateDto
        {
            StorageId = s.StorageId,
            UserId = s.UserId,
            TemplateId = s.TemplateId,
            UploadDate = s.UploadDate,
            UpdatedDate = s.UpdatedDate,
            Status = s.Status,
            UserName = s.UserName,
            TemplateName = s.TemplateName,
            TemplateDescription = s.TemplateDescription,
            TemplatePrice = s.TemplatePrice,
            TemplatePreviewUrl = s.TemplatePreviewUrl
        });

        return Ok(storageDtos);
    }

    /// <summary>
    /// [AUTH] Add template to storage/library (after purchase)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToStorage([FromBody] StorageTemplateCreateDto storageDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var createServiceDto = new StorageTemplateCreateServiceDto
            {
                TemplateId = storageDto.TemplateId
            };

            var storage = await _storageService.AddToStorageAsync(userId, createServiceDto);
            
            var resultDto = new StorageTemplateDto
            {
                StorageId = storage.StorageId,
                UserId = storage.UserId,
                TemplateId = storage.TemplateId,
                UploadDate = storage.UploadDate,
                UpdatedDate = storage.UpdatedDate,
                Status = storage.Status,
                TemplateName = storage.TemplateName,
                TemplateDescription = storage.TemplateDescription,
                TemplatePrice = storage.TemplatePrice,
                TemplatePreviewUrl = storage.TemplatePreviewUrl
            };

            return CreatedAtAction(nameof(GetMyStorage), resultDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// [AUTH] Remove template from storage/library
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromStorage(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _storageService.RemoveFromStorageAsync(id, userId);
        
        if (!result)
            return NotFound(new { message = $"Storage item with ID {id} not found" });

        return NoContent();
        // UnauthorizedAccessException handled by middleware
    }

    /// <summary>
    /// [AUTH] Check if template is in storage
    /// </summary>
    [HttpGet("check/{templateId}")]
    public async Task<IActionResult> CheckStorage(int templateId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isInStorage = await _storageService.IsInStorageAsync(userId, templateId);
        
        return Ok(new { templateId, isInStorage });
    }
} 
