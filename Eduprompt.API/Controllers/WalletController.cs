using Eduprompt.Domain.DTOs.Wallet;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "12. Wallet")]
[Produces("application/json")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    /// <summary>
    /// Get wallet by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User's wallet details</returns>
    /// <response code="200">Wallet found</response>
    /// <response code="400">Error retrieving wallet</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Wallet not found</response>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var wallet = await _walletService.GetByUserIdAsync(userId);
            if (wallet == null)
                return NotFound(new { message = "Wallet not found" });

            return Ok(wallet);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get wallet by wallet ID
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <returns>Wallet details</returns>
    /// <response code="200">Wallet found</response>
    /// <response code="400">Error retrieving wallet</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Wallet not found</response>
    [HttpGet("{walletId}")]
    public async Task<IActionResult> GetById(int walletId)
    {
        try
        {
            var wallet = await _walletService.GetByIdAsync(walletId);
            if (wallet == null)
                return NotFound(new { message = "Wallet not found" });

            return Ok(wallet);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo ví mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWalletDto createWalletDto)
    {
        try
        {
            var wallet = await _walletService.CreateAsync(createWalletDto);
            return CreatedAtAction(nameof(GetById), new { walletId = wallet.WalletID }, wallet);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật ví
    /// </summary>
    [HttpPut("{walletId}")]
    public async Task<IActionResult> Update(int walletId, [FromBody] UpdateWalletDto updateWalletDto)
    {
        try
        {
            var wallet = await _walletService.UpdateAsync(walletId, updateWalletDto);
            return Ok(wallet);
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
    /// Xóa ví
    /// </summary>
    [HttpDelete("{walletId}")]
    public async Task<IActionResult> Delete(int walletId)
    {
        try
        {
            var result = await _walletService.DeleteAsync(walletId);
            if (!result)
                return NotFound(new { message = "Wallet not found" });

            return Ok(new { message = "Wallet deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy số dư ví
    /// </summary>
    [HttpGet("balance/{userId}")]
    public async Task<IActionResult> GetBalance(int userId)
    {
        try
        {
            var balance = await _walletService.GetBalanceAsync(userId);
            return Ok(new { balance });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Nạp tiền vào ví
    /// </summary>
    [HttpPost("add-funds")]
    public async Task<IActionResult> AddFunds([FromBody] AddFundsRequest request)
    {
        try
        {
            var result = await _walletService.AddFundsAsync(request.UserId, request.Amount);
            if (!result)
                return BadRequest(new { message = "Failed to add funds" });

            return Ok(new { message = "Funds added successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Trừ tiền từ ví
    /// </summary>
    [HttpPost("deduct-funds")]
    public async Task<IActionResult> DeductFunds([FromBody] DeductFundsRequest request)
    {
        try
        {
            var result = await _walletService.DeductFundsAsync(request.UserId, request.Amount);
            if (!result)
                return BadRequest(new { message = "Insufficient funds or user not found" });

            return Ok(new { message = "Funds deducted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class AddFundsRequest
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}

public class DeductFundsRequest
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}
