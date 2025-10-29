using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/templates")]
[ApiExplorerSettings(GroupName = "22. TemplateCommerce")]
[Produces("application/json")]
public class TemplateCommerceController : ControllerBase
{
    private readonly ITemplateCommerceService _commerce;

    public TemplateCommerceController(ITemplateCommerceService commerce)
    {
        _commerce = commerce;
    }

    public sealed class PurchaseRequest
    {
        public string Mode { get; set; } = "direct"; // with_ai | direct
        public decimal Price { get; set; }
    }

    /// <summary>
    /// Purchase a template architecture and grant ownership to current user
    /// </summary>
    /// <param name="templateArchitectureId">Template architecture ID</param>
    /// <param name="request">Purchase options: mode (with_ai|direct), price</param>
    /// <returns>StorageId and PromptInstanceId created for buyer</returns>
    /// <response code="200">Purchase completed</response>
    /// <response code="400">Validation or processing error</response>
    [HttpPost("{templateArchitectureId}/purchase")]
    [Authorize]
    public async Task<IActionResult> Purchase(int templateArchitectureId, [FromBody] PurchaseRequest request)
    {
        try
        {
            var buyerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var result = await _commerce.PurchaseTemplateAsync(buyerId, templateArchitectureId, request.Mode, request.Price);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}


