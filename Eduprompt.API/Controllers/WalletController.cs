using Eduprompt.Domain.DTOs.Wallet;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/wallets")]
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
    /// <param name="UserId">User ID</param>
    /// <returns>User's wallet details</returns>
    /// <response code="200">Wallet found</response>
    /// <response code="400">Error retrieving wallet</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Wallet not found</response>
    [HttpGet("user/{UserId}")]
    public async Task<IActionResult> GetByUserId(int UserId)
    {
        try
        {
            var wallet = await _walletService.GetByUserIdAsync(UserId);
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
    /// <param name="WalletId">Wallet ID</param>
    /// <returns>Wallet details</returns>
    /// <response code="200">Wallet found</response>
    /// <response code="400">Error retrieving wallet</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Wallet not found</response>
    [HttpGet("{WalletId}")]
    public async Task<IActionResult> GetById(int WalletId)
    {
        try
        {
            var wallet = await _walletService.GetByIdAsync(WalletId);
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
            return CreatedAtAction(nameof(GetById), new { WalletId = wallet.WalletId }, wallet);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật ví
    /// </summary>
    [HttpPut("{WalletId}")]
    public async Task<IActionResult> Update(int WalletId, [FromBody] UpdateWalletDto updateWalletDto)
    {
        try
        {
            var wallet = await _walletService.UpdateAsync(WalletId, updateWalletDto);
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
    [HttpDelete("{WalletId}")]
    public async Task<IActionResult> Delete(int WalletId)
    {
        try
        {
            var result = await _walletService.DeleteAsync(WalletId);
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
    [HttpGet("balance/{UserId}")]
    public async Task<IActionResult> GetBalance(int UserId)
    {
        try
        {
            var balance = await _walletService.GetBalanceByUserIdAsync(UserId);
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
            var result = await _walletService.AddFundsByUserIdAsync(request.UserId, request.Amount);
            if (!result)
                return BadRequest(new { message = "Failed to add funds" });

            return Ok(new { message = "Funds added successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
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
            var result = await _walletService.DeductFundsByUserIdAsync(request.UserId, request.Amount);
            if (!result)
                return BadRequest(new { message = "Insufficient funds or user not found" });

            return Ok(new { message = "Funds deducted successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
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
