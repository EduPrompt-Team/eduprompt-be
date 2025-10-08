using Eduprompt.Domain.DTOs.PaymentMethod;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// 💳 Payment Methods - Quản lý phương thức thanh toán
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    /// Lấy danh sách phương thức thanh toán của user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var paymentMethods = await _paymentMethodService.GetByUserIdAsync(userId);
            return Ok(paymentMethods);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết phương thức thanh toán
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var paymentMethod = await _paymentMethodService.GetByIdAsync(id);
            if (paymentMethod == null)
                return NotFound();

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
            var paymentMethod = await _paymentMethodService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = paymentMethod.PaymentMethodID }, paymentMethod);
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
            var paymentMethod = await _paymentMethodService.UpdateAsync(id, updateDto);
            return Ok(paymentMethod);
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
                return NotFound();

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
    [HttpGet("user/{userId}/default")]
    public async Task<IActionResult> GetDefault(int userId)
    {
        try
        {
            var paymentMethod = await _paymentMethodService.GetDefaultByUserIdAsync(userId);
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
    public async Task<IActionResult> SetAsDefault(int id, [FromQuery] int userId)
    {
        try
        {
            var result = await _paymentMethodService.SetAsDefaultAsync(id, userId);
            if (!result)
                return NotFound();

            return Ok(new { message = "Set as default successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
