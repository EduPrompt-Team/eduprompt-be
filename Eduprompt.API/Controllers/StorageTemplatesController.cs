using Eduprompt.Domain.DTOs.StorageTemplate;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var storage = await _storageService.GetUserStorageAsync(userId);
        return Ok(storage);
    }

    [HttpPost]
    public async Task<IActionResult> AddToStorage([FromBody] StorageTemplateCreateDto storageDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var created = await _storageService.AddToStorageAsync(userId, new StorageTemplateCreateServiceDto
        {
            TemplateId = storageDto.PackageID
        });
        return CreatedAtAction(nameof(GetMyStorage), created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromStorage(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _storageService.RemoveFromStorageAsync(id, userId);
        if (!result)
            return NotFound(new { message = $"Storage item with ID {id} not found" });
        return NoContent();
    }

    [HttpGet("check/{packageId}")]
    public async Task<IActionResult> CheckStorage(int packageId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isInStorage = await _storageService.IsInStorageAsync(userId, packageId);
        return Ok(new { packageId, isInStorage });
    }
} 
