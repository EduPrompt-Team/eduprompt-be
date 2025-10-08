using Eduprompt.Domain.DTOs.Transaction;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// 💰 Transactions - Quản lý giao dịch
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "11. Transactions")]
[Produces("application/json")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Lấy danh sách giao dịch theo ví
    /// </summary>
    [HttpGet("wallet/{walletId}")]
    public async Task<IActionResult> GetByWalletId(int walletId)
    {
        try
        {
            var transactions = await _transactionService.GetByWalletIdAsync(walletId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách giao dịch theo user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var transactions = await _transactionService.GetByUserIdAsync(userId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết giao dịch
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo giao dịch mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto createDto)
    {
        try
        {
            var transaction = await _transactionService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = transaction.TransactionID }, transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật giao dịch
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateTransactionDto updateDto)
    {
        try
        {
            var transaction = await _transactionService.UpdateAsync(id, updateDto);
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa giao dịch
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _transactionService.DeleteAsync(id);
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
    /// Lấy số dư ví
    /// </summary>
    [HttpGet("wallet/{walletId}/balance")]
    public async Task<IActionResult> GetWalletBalance(int walletId)
    {
        try
        {
            var balance = await _transactionService.GetWalletBalanceAsync(walletId);
            return Ok(new { balance });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy giao dịch gần đây
    /// </summary>
    [HttpGet("wallet/{walletId}/recent")]
    public async Task<IActionResult> GetRecent(int walletId, [FromQuery] int count = 20)
    {
        try
        {
            var transactions = await _transactionService.GetRecentTransactionsAsync(walletId, count);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
