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
        // Temporarily use default UserId for testing since authorize is disabled
        var UserId = 1; // Default user ID for testing
        // var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var storage = await _storageService.GetUserStorageAsync(UserId);
        return Ok(storage);
    }

    [HttpPost]
    public async Task<IActionResult> AddToStorage([FromBody] StorageTemplateCreateDto storageDto)
    {
        // Temporarily use default UserId for testing since authorize is disabled
        var UserId = 1; // Default user ID for testing
        // var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var created = await _storageService.AddToStorageAsync(UserId, new StorageTemplateCreateServiceDto
        {
            TemplateId = storageDto.PackageId
        });
        return CreatedAtAction(nameof(GetMyStorage), created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromStorage(int id)
    {
        // Temporarily use default UserId for testing since authorize is disabled
        var UserId = 1; // Default user ID for testing
        // var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _storageService.RemoveFromStorageAsync(id, UserId);
        if (!result)
            return NotFound(new { message = $"Storage item with ID {id} not found" });
        return NoContent();
    }

    [HttpGet("check/{PackageId}")]
    public async Task<IActionResult> CheckStorage(int PackageId)
    {
        // Temporarily use default UserId for testing since authorize is disabled
        var UserId = 1; // Default user ID for testing
        // var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isInStorage = await _storageService.IsInStorageAsync(UserId, PackageId);
        return Ok(new { PackageId, isInStorage });
    }
} 
