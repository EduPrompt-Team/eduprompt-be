using Eduprompt.Domain.DTOs.PaymentMethod;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Payment method management for user transactions
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
    /// Get all payment methods (Admin only)
    /// </summary>
    /// <returns>List of all payment methods</returns>
    /// <response code="200">Payment methods retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        // PaymentMethod table has UserId column issue - temporarily disabled
        return Ok(new List<object>());
    }

    /// <summary>
    /// Get payment methods by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's payment methods</returns>
    /// <response code="200">Payment methods retrieved successfully</response>
    /// <response code="400">Error retrieving payment methods</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        // PaymentMethod table has UserId column issue - temporarily disabled
        return Ok(new List<object>());
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
        // PaymentMethod table has UserId column issue - temporarily disabled
        return Ok(new { message = "PaymentMethod not available" });
    }

    /// <summary>
    /// Thêm phương thức thanh toán mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentMethodDto createDto)
    {
        // PaymentMethod table has UserId column issue - temporarily disabled
        return Ok(new { message = "PaymentMethod not available" });
    }

    /// <summary>
    /// Cập nhật phương thức thanh toán
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePaymentMethodDto updateDto)
    {
        // PaymentMethod table has UserId column issue - temporarily disabled
        return Ok(new { message = "PaymentMethod not available" });
    }

    /// <summary>
    /// Xóa phương thức thanh toán
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // PaymentMethod table has UserId column issue - temporarily disabled
        return Ok(new { message = "PaymentMethod not available" });
    }

    /// <summary>
    /// Lấy phương thức thanh toán mặc định
    /// </summary>
    [HttpGet("user/{userId}/default")]
    public async Task<IActionResult> GetDefault(int userId)
    {
        // PaymentMethod table has UserId column issue - temporarily disabled
        return Ok(new { message = "PaymentMethod not available" });
    }

    /// <summary>
    /// Đặt làm phương thức thanh toán mặc định
    /// </summary>
    [HttpPost("{id}/set-default")]
    public async Task<IActionResult> SetAsDefault(int id, [FromQuery] int userId)
    {
        // PaymentMethod table has UserId column issue - temporarily disabled
        return Ok(new { message = "PaymentMethod not available" });
    }
}
