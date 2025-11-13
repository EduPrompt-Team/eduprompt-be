using Eduprompt.Domain.DTOs.PromptInstance;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/prompt-instances")]
[ApiExplorerSettings(GroupName = "20. Prompt Instance")]
[Produces("application/json")]
[Authorize]
public class PromptInstanceController : ControllerBase
{
    private readonly IPromptInstanceService _promptInstanceService;

    public PromptInstanceController(IPromptInstanceService promptInstanceService)
    {
        _promptInstanceService = promptInstanceService;
    }

    /// <summary>
    /// Get prompt instance by ID (only owner or admin)
    /// </summary>
    /// <param name="InstanceId">Prompt instance ID</param>
    /// <returns>Prompt instance details</returns>
    /// <response code="200">Instance found</response>
    /// <response code="400">Error retrieving instance</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized</response>
    /// <response code="404">Instance not found</response>
    [HttpGet("{InstanceId}")]
    public async Task<IActionResult> GetById(int InstanceId)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            var instance = await _promptInstanceService.GetByIdAsync(InstanceId);
            if (instance == null)
                return NotFound(new { message = "Prompt instance not found" });

            // Only allow viewing own instances unless admin
            if (instance.UserId != userId && !User.IsInRole("Admin"))
                return StatusCode(403, new { message = "You can only view your own instances" });

            return Ok(instance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get prompt instances by user ID (only own instances)
    /// </summary>
    /// <returns>List of current user's prompt instances</returns>
    /// <response code="200">Instances retrieved successfully</response>
    /// <response code="400">Error retrieving instances</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("my-instances")]
    public async Task<IActionResult> GetMyInstances()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            var instances = await _promptInstanceService.GetByUserIdAsync(userId);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get prompt instances by user ID (Admin only or own instances)
    /// </summary>
    [HttpGet("user/{UserId}")]
    public async Task<IActionResult> GetByUserId(int UserId)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            // Only allow viewing own instances unless admin
            if (UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return StatusCode(403, new { message = "You can only view your own instances" });
            }

            var instances = await _promptInstanceService.GetByUserIdAsync(UserId);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy instances theo Template ID (PackageId)
    /// Note: templateId in this endpoint refers to PackageId
    /// For StorageTemplate-based queries, use /storage/{storageId} endpoint
    /// </summary>
    [HttpGet("template/{templateId}")]
    public async Task<IActionResult> GetByTemplateId(int templateId)
    {
        try
        {
            var instances = await _promptInstanceService.GetByTemplateIdAsync(templateId);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy instances theo StorageTemplate ID (StorageId)
    /// Note: Returns ALL instances with matching PackageId, not filtered by UserId
    /// For user-specific instances, use /storage/{storageId}/my endpoint
    /// </summary>
    [HttpGet("storage/{storageId}")]
    public async Task<IActionResult> GetByStorageId(int storageId)
    {
        try
        {
            var instances = await _promptInstanceService.GetByStorageIdAsync(storageId);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy instances theo StorageTemplate ID (StorageId) của user hiện tại
    /// </summary>
    [HttpGet("storage/{storageId}/my")]
    public async Task<IActionResult> GetMyInstancesByStorageId(int storageId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var instances = await _promptInstanceService.GetByStorageIdAndUserIdAsync(storageId, userId);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy instances theo trạng thái
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        try
        {
            var instances = await _promptInstanceService.GetByStatusAsync(status);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy instances gần đây của user
    /// </summary>
    [HttpGet("recent/{UserId}")]
    public async Task<IActionResult> GetRecentInstances(int UserId, [FromQuery] int count = 10)
    {
        try
        {
            var instances = await _promptInstanceService.GetRecentInstancesAsync(UserId, count);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo instance mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePromptInstanceDto createPromptInstanceDto)
    {
        try
        {
            // Verify userId from token matches request
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            // Override UserId from token to prevent user creating instance for others
            createPromptInstanceDto.UserId = userId;

            var instance = await _promptInstanceService.CreateAsync(createPromptInstanceDto);
            return CreatedAtAction(nameof(GetById), new { InstanceId = instance.InstanceId }, instance);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new 
            { 
                message = ex.Message,
                errors = new Dictionary<string, string[]>
                {
                    { "packageId", new[] { ex.Message } },
                    { "storageId", new[] { ex.Message } }
                }
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật instance (only owner or admin)
    /// </summary>
    [HttpPut("{InstanceId}")]
    public async Task<IActionResult> Update(int InstanceId, [FromBody] UpdatePromptInstanceDto updatePromptInstanceDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            // Verify ownership
            var existingInstance = await _promptInstanceService.GetByIdAsync(InstanceId);
            if (existingInstance == null)
                return NotFound(new { message = "Prompt instance not found" });

            if (existingInstance.UserId != userId && !User.IsInRole("Admin"))
                return StatusCode(403, new { message = "You can only update your own instances" });

            var instance = await _promptInstanceService.UpdateAsync(InstanceId, updatePromptInstanceDto);
            return Ok(instance);
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
    /// Xóa instance (only owner or admin)
    /// </summary>
    [HttpDelete("{InstanceId}")]
    public async Task<IActionResult> Delete(int InstanceId)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            // Verify ownership
            var existingInstance = await _promptInstanceService.GetByIdAsync(InstanceId);
            if (existingInstance == null)
                return NotFound(new { message = "Prompt instance not found" });

            if (existingInstance.UserId != userId && !User.IsInRole("Admin"))
                return StatusCode(403, new { message = "You can only delete your own instances" });

            var result = await _promptInstanceService.DeleteAsync(InstanceId);
            if (!result)
                return NotFound(new { message = "Prompt instance not found" });

            return Ok(new { message = "Prompt instance deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Hoàn thành instance với output data (only owner or admin)
    /// </summary>
    [HttpPost("{InstanceId}/complete")]
    public async Task<IActionResult> CompleteInstance(int InstanceId, [FromBody] CompletePromptInstanceDto completeDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            // Verify ownership
            var existingInstance = await _promptInstanceService.GetByIdAsync(InstanceId);
            if (existingInstance == null)
                return NotFound(new { message = "Prompt instance not found" });

            if (existingInstance.UserId != userId && !User.IsInRole("Admin"))
                return StatusCode(403, new { message = "You can only complete your own instances" });

            // Log received data for debugging
            var outputJsonLength = completeDto.OutputJson?.Length ?? 0;
            var hasOutputJson = !string.IsNullOrEmpty(completeDto.OutputJson);
            
            var instance = await _promptInstanceService.CompleteAsync(InstanceId, completeDto);
            
            // Verify outputJson was saved
            if (hasOutputJson && string.IsNullOrEmpty(instance.OutputJson))
            {
                // Log warning if outputJson was provided but not saved
                return BadRequest(new 
                { 
                    message = "OutputJson was provided but not saved. Please check backend logs.",
                    receivedOutputJsonLength = outputJsonLength,
                    savedOutputJsonLength = instance.OutputJson?.Length ?? 0
                });
            }
            
            return Ok(instance);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
