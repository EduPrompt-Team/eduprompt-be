using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/payments")]
[ApiExplorerSettings(GroupName = "13. Payments")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _paymentService.GetAllPaymentsAsync());
    }

    [HttpGet("{paymentId}")]
    [Authorize]
    public async Task<IActionResult> GetById(int paymentId)
    {
        var p = await _paymentService.GetByIdAsync(paymentId);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpGet("orders/{orderId}")]
    [Authorize]
    public async Task<IActionResult> GetByOrder(int orderId)
    {
        return Ok(await _paymentService.GetByOrderIdAsync(orderId));
    }

    [HttpPost("orders/{orderId}/vnpay-url")]
    [Authorize]
    public async Task<IActionResult> CreateVnpayUrl(int orderId, [FromBody] VnpayRequestServiceDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        dto.IpAddr ??= HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var url = await _paymentService.CreateVnpayPaymentUrlAsync(orderId, userId, dto);
        return Ok(new { url });
    }

    [HttpGet("vnpay-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> VnpayCallback([FromQuery] VnpayCallbackServiceDto cb)
    {
        var result = await _paymentService.ProcessVnpayCallbackAsync(cb);
        return Ok(result);
    }

    [HttpPost("querydr")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Query([FromBody] VnpayQueryRequestDto dto)
    {
        dto.IpAddr ??= HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var result = await _paymentService.QueryVnpayTransactionAsync(dto);
        return Ok(result);
    }

    [HttpPost("refund")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Refund([FromBody] VnpayRefundRequestDto dto)
    {
        dto.IpAddr ??= HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var result = await _paymentService.RefundVnpayTransactionAsync(dto);
        return Ok(result);
    }

    // IPN (Instant Payment Notification) - server to server
    [HttpPost("vnpay-ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> VnpayIpn([FromForm] VnpayCallbackServiceDto cb)
    {
        try
        {
            var _ = await _paymentService.ProcessVnpayCallbackAsync(cb);
            return Ok(new { RspCode = "00", Message = "Confirm Success" });
        }
        catch
        {
            return Ok(new { RspCode = "97", Message = "Invalid signature or data" });
        }
    }

    [HttpPost("orders/{orderId}/manual")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ManualPayment(int orderId, [FromBody] PaymentCreateServiceDto dto)
    {
        var result = await _paymentService.CreateManualPaymentAsync(orderId, dto);
        return Ok(result);
    }

    [HttpPatch("{paymentId}/status")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateStatus(int paymentId, [FromQuery] string status)
    {
        var result = await _paymentService.UpdatePaymentStatusAsync(paymentId, status);
        return Ok(result);
    }
}


