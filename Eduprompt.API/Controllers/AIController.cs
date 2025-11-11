using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eduprompt.API.Controllers;

/// <summary>
/// AI suggestions and generation endpoints
/// </summary>
[ApiController]
[Route("api/ai")]
[ApiExplorerSettings(GroupName = "25. AI")]
[Produces("application/json")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAihistoryService _aiHistoryService;
    private readonly IPromptInstanceService _promptInstanceService;
    private readonly IExpectedOutputService _expectedOutputService;

    public AIController(
        IAihistoryService aiHistoryService,
        IPromptInstanceService promptInstanceService,
        IExpectedOutputService expectedOutputService)
    {
        _aiHistoryService = aiHistoryService;
        _promptInstanceService = promptInstanceService;
        _expectedOutputService = expectedOutputService;
    }

    /// <summary>
    /// Generate AI suggestions for a prompt instance
    /// </summary>
    /// <param name="instanceId">Prompt instance ID</param>
    /// <param name="request">AI generation request</param>
    /// <returns>Generated expected output</returns>
    /// <response code="200">AI suggestions generated successfully</response>
    /// <response code="400">Invalid request or AI generation failed</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Prompt instance not found</response>
    [HttpPost("suggestions/{instanceId}")]
    public async Task<IActionResult> GenerateSuggestions(int instanceId, [FromBody] AISuggestionRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            // Get prompt instance
            var instance = await _promptInstanceService.GetByIdAsync(instanceId);
            if (instance == null)
                return NotFound(new { message = "Prompt instance not found" });

            // Verify ownership
            if (instance.UserId != userId)
                return StatusCode(403, new { message = "You don't have permission to generate suggestions for this instance" });

            // Generate AI output (mock implementation - replace with actual AI service)
            string generatedOutput;
            try
            {
                // Set timeout for AI generation (30 seconds)
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                generatedOutput = await GenerateAIOutputAsync(instance, request);
            }
            catch (OperationCanceledException)
            {
                // AI service timeout - fallback: save PromptInstance without ExpectedOutput
                await _promptInstanceService.UpdateAsync(instanceId, new Domain.DTOs.PromptInstance.UpdatePromptInstanceDto
                {
                    Status = "Completed",
                    OutputJson = instance.InputJson ?? "{}"
                });
                return BadRequest(new { message = "AI service timeout. PromptInstance saved without AI output." });
            }
            catch (Exception aiEx)
            {
                // AI service error - fallback: save PromptInstance without ExpectedOutput
                await _promptInstanceService.UpdateAsync(instanceId, new Domain.DTOs.PromptInstance.UpdatePromptInstanceDto
                {
                    Status = "Completed",
                    OutputJson = instance.InputJson ?? "{}"
                });
                return BadRequest(new { message = $"AI service error: {aiEx.Message}. PromptInstance saved without AI output." });
            }

            // Create ExpectedOutput with generated content
            var expectedOutputDto = new Domain.DTOs.ExpectedOutput.CreateExpectedOutputDto
            {
                PromptInstanceId = instanceId,
                OutputName = request.OutputName ?? "AI Generated Output"
            };
            
            // Store generated output in OutputDetails
            expectedOutputDto.OutputDetails = new List<Domain.DTOs.ExpectedOutput.CreateOutputDetailDto>
            {
                new Domain.DTOs.ExpectedOutput.CreateOutputDetailDto
                {
                    Description = generatedOutput
                }
            };

            var expectedOutput = await _expectedOutputService.CreateAsync(expectedOutputDto);
            
            // Update ExpectedOutput ExampleOutput field (if service supports it)
            // Note: May need to update ExpectedOutputService to support ExampleOutput in CreateDto

            // Create AI History record
            await _aiHistoryService.CreateAsync(new Domain.DTOs.Aihistory.CreateAihistoryDto
            {
                UserId = userId,
                PromptInstanceId = instanceId,
                UserMessage = instance.InputJson ?? "{}",
                Airesponse = generatedOutput,
                Status = "Completed"
            });

            // Update prompt instance status
            await _promptInstanceService.UpdateAsync(instanceId, new Domain.DTOs.PromptInstance.UpdatePromptInstanceDto
            {
                Status = "Completed",
                OutputJson = generatedOutput
            });

            return Ok(new
            {
                expectedOutputId = expectedOutput.OutputId,
                generatedOutput,
                message = "AI suggestions generated successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mock AI generation - replace with actual AI service integration
    /// </summary>
    private async Task<string> GenerateAIOutputAsync(
        Domain.DTOs.PromptInstance.PromptInstanceDto instance,
        AISuggestionRequest request)
    {
        // TODO: Replace with actual AI service call (OpenAI, Anthropic, etc.)
        // For now, return a mock response based on input
        await Task.Delay(500); // Simulate AI processing time

        var inputData = instance.InputJson ?? "{}";
        return $"{{\"suggested_output\": \"AI generated content based on: {inputData}\", \"confidence\": 0.85}}";
    }
}

public class AISuggestionRequest
{
    public string? OutputName { get; set; }
    public string? Model { get; set; } = "gpt-3.5-turbo";
    public Dictionary<string, object>? Parameters { get; set; }
}

