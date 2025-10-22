using Eduprompt.Domain.DTOs.PromptInstance;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "17. Prompt Instance")]
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
    /// Get prompt instance by ID
    /// </summary>
    /// <param name="InstanceId">Prompt instance ID</param>
    /// <returns>Prompt instance details</returns>
    /// <response code="200">Instance found</response>
    /// <response code="400">Error retrieving instance</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Instance not found</response>
    [HttpGet("{InstanceId}")]
    public async Task<IActionResult> GetById(int InstanceId)
    {
        try
        {
            var instance = await _promptInstanceService.GetByIdAsync(InstanceId);
            if (instance == null)
                return NotFound(new { message = "Prompt instance not found" });

            return Ok(instance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get prompt instances by user ID
    /// </summary>
    /// <param name="UserId">User ID</param>
    /// <returns>List of user's prompt instances</returns>
    /// <response code="200">Instances retrieved successfully</response>
    /// <response code="400">Error retrieving instances</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("user/{UserId}")]
    public async Task<IActionResult> GetByUserId(int UserId)
    {
        try
        {
            var instances = await _promptInstanceService.GetByUserIdAsync(UserId);
            return Ok(instances);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy instances theo Template ID
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
            var instance = await _promptInstanceService.CreateAsync(createPromptInstanceDto);
            return CreatedAtAction(nameof(GetById), new { InstanceId = instance.InstanceId }, instance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật instance
    /// </summary>
    [HttpPut("{InstanceId}")]
    public async Task<IActionResult> Update(int InstanceId, [FromBody] UpdatePromptInstanceDto updatePromptInstanceDto)
    {
        try
        {
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
    /// Xóa instance
    /// </summary>
    [HttpDelete("{InstanceId}")]
    public async Task<IActionResult> Delete(int InstanceId)
    {
        try
        {
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
    /// Hoàn thành instance với output data
    /// </summary>
    [HttpPost("{InstanceId}/complete")]
    public async Task<IActionResult> CompleteInstance(int InstanceId, [FromBody] CompleteInstanceRequest request)
    {
        try
        {
            var result = await _promptInstanceService.CompleteInstanceAsync(InstanceId, request.OutputData);
            if (!result)
                return NotFound(new { message = "Prompt instance not found" });

            return Ok(new { message = "Instance completed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class CompleteInstanceRequest
{
    public string OutputData { get; set; } = string.Empty;
}
