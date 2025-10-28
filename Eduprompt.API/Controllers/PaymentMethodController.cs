using Eduprompt.Domain.DTOs.PaymentMethod;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Payment method management for user transactions
/// </summary>
[ApiController]
[Route("api/payment-methods")]
[ApiExplorerSettings(GroupName = "10. Payment Methods")]
[Produces("application/json")]
[Authorize]
public class PaymentMethodController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodController(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    /// <summary>
    /// Get all payment methods (Admin only)
    /// </summary>
    /// <returns>List of all payment methods</returns>
    /// <response code="200">Payment methods retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var paymentMethods = await _paymentMethodService.GetAllAsync();
            return Ok(paymentMethods);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get payment methods by user ID
    /// </summary>
    /// <param name="UserId">User ID</param>
    /// <returns>List of user's payment methods</returns>
    /// <response code="200">Payment methods retrieved successfully</response>
    /// <response code="400">Error retrieving payment methods</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("user/{UserId}")]
    public async Task<IActionResult> GetByUserId(int UserId)
    {
        try
        {
            var paymentMethods = await _paymentMethodService.GetByUserIdAsync(UserId);
            return Ok(paymentMethods);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get payment method by ID
    /// </summary>
    /// <param name="id">Payment method ID</param>
    /// <returns>Payment method details</returns>
    /// <response code="200">Payment method found</response>
    /// <response code="400">Error retrieving payment method</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Payment method not found</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var paymentMethod = await _paymentMethodService.GetByIdAsync(id);
            if (paymentMethod == null)
                return NotFound(new { message = "Payment method not found" });
            
            return Ok(paymentMethod);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Thêm phương thức thanh toán mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentMethodDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var paymentMethod = await _paymentMethodService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = paymentMethod.PaymentMethodId }, paymentMethod);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật phương thức thanh toán
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePaymentMethodDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var paymentMethod = await _paymentMethodService.UpdateAsync(id, updateDto);
            return Ok(paymentMethod);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Payment method not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa phương thức thanh toán
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _paymentMethodService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Payment method not found" });
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy phương thức thanh toán mặc định
    /// </summary>
    [HttpGet("user/{UserId}/default")]
    public async Task<IActionResult> GetDefault(int UserId)
    {
        try
        {
            var paymentMethod = await _paymentMethodService.GetDefaultByUserIdAsync(UserId);
            if (paymentMethod == null)
                return NotFound(new { message = "No default payment method found" });
            
            return Ok(paymentMethod);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Đặt làm phương thức thanh toán mặc định
    /// </summary>
    [HttpPost("{id}/set-default")]
    public async Task<IActionResult> SetAsDefault(int id, [FromQuery] int UserId)
    {
        try
        {
            var result = await _paymentMethodService.SetAsDefaultAsync(id, UserId);
            if (!result)
                return NotFound(new { message = "Payment method not found" });
            
            return Ok(new { message = "Payment method set as default successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
