using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    /// <summary>
    /// Tạo VNPay payment URL cho wallet top-up (nạp tiền vào ví)
    /// </summary>
    /// <param name="walletId">ID của wallet cần nạp tiền</param>
    /// <param name="dto">Thông tin nạp tiền</param>
    /// <returns>VNPay payment URL</returns>
    /// <response code="200">Trả về payment URL thành công</response>
    /// <response code="400">Amount <= 0 hoặc wallet không hợp lệ</response>
    /// <response code="401">User chưa đăng nhập</response>
    /// <response code="403">Wallet không thuộc về user hiện tại</response>
    /// <response code="404">Wallet không tồn tại</response>
    [HttpPost("wallets/{walletId}/topup")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateVnpayUrlForWalletTopup(int walletId, [FromBody] WalletTopupRequestDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var requestDto = new VnpayRequestServiceDto
        {
            BankCode = dto.BankCode,
            Language = dto.Language ?? "vn",
            ReturnUrl = dto.ReturnUrl,
            IpAddr = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
        };
        var url = await _paymentService.CreateVnpayUrlForWalletTopupAsync(walletId, dto.Amount, userId, requestDto);
        return Ok(new { url });
    }

    /// <summary>
    /// Tạo VNPay payment URL cho transaction payment
    /// </summary>
    /// <param name="transactionId">ID của transaction cần thanh toán</param>
    /// <param name="dto">Thông tin thanh toán VNPay</param>
    /// <returns>VNPay payment URL</returns>
    /// <response code="200">Trả về payment URL thành công</response>
    /// <response code="400">Transaction không hợp lệ</response>
    /// <response code="401">User chưa đăng nhập</response>
    /// <response code="403">Transaction không thuộc về user hiện tại</response>
    /// <response code="404">Transaction không tồn tại</response>
    [HttpPost("transactions/{transactionId}/vnpay-url")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateVnpayUrlForTransaction(int transactionId, [FromBody] VnpayRequestServiceDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        dto.IpAddr ??= HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var url = await _paymentService.CreateVnpayUrlForTransactionAsync(transactionId, userId, dto);
        return Ok(new { url });
    }

    /// <summary>
    /// Kiểm tra trạng thái thanh toán của một package cho user hiện tại
    /// </summary>
    /// <param name="packageId">ID của package cần kiểm tra</param>
    /// <returns>Thông tin trạng thái thanh toán</returns>
    /// <response code="200">Trả về thông tin payment status (isPaid có thể là true hoặc false)</response>
    /// <response code="401">User chưa đăng nhập</response>
    [HttpGet("check-package/{packageId}")]
    [Authorize]
    [ProducesResponseType(typeof(PackagePaymentStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckPackagePayment(int packageId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var status = await _paymentService.CheckPackagePaymentAsync(packageId, userId);
        return Ok(status);
    }
}

// DTO for wallet top-up request
/// <summary>
/// Request DTO for wallet top-up payment
/// </summary>
public class WalletTopupRequestDto
{
    /// <summary>
    /// Số tiền nạp (VND), phải > 0
    /// </summary>
    /// <example>100000</example>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Mã ngân hàng (optional), ví dụ: NCB, VIETCOMBANK
    /// </summary>
    /// <example>NCB</example>
    public string? BankCode { get; set; }
    
    /// <summary>
    /// Ngôn ngữ (optional), mặc định: "vn"
    /// </summary>
    /// <example>vn</example>
    public string? Language { get; set; }
    
    /// <summary>
    /// URL callback sau khi thanh toán (optional), override default ReturnUrl
    /// </summary>
    /// <example>https://yourapp.com/payment/callback</example>
    public string? ReturnUrl { get; set; }
}


