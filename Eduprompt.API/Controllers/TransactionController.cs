using Eduprompt.Domain.DTOs.Transaction;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Transaction management for wallet operations
/// </summary>
[ApiController]
[Route("api/transactions")]
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
    /// Get all transactions (Admin only)
    /// </summary>
    /// <returns>List of all transactions</returns>
    /// <response code="200">Transactions retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var transactions = await _transactionService.GetAllAsync();
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get transactions by wallet ID
    /// </summary>
    /// <param name="WalletId">Wallet ID</param>
    /// <returns>List of transactions for the wallet</returns>
    /// <response code="200">Transactions retrieved successfully</response>
    /// <response code="400">Error retrieving transactions</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("wallet/{WalletId}")]
    public async Task<IActionResult> GetByWalletId(int WalletId)
    {
        try
        {
            var transactions = await _transactionService.GetByWalletIdAsync(WalletId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get transactions by user ID
    /// </summary>
    /// <param name="UserId">User ID</param>
    /// <returns>List of transactions for the user</returns>
    /// <response code="200">Transactions retrieved successfully</response>
    /// <response code="400">Error retrieving transactions</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("user/{UserId}")]
    public async Task<IActionResult> GetByUserId(int UserId)
    {
        try
        {
            var transactions = await _transactionService.GetByUserIdAsync(UserId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    /// <param name="id">Transaction ID</param>
    /// <returns>Transaction details</returns>
    /// <response code="200">Transaction found</response>
    /// <response code="400">Error retrieving transaction</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Transaction not found</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
                return NotFound(new { message = "Transaction not found" });
            
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create new transaction
    /// </summary>
    /// <param name="createDto">Transaction creation data</param>
    /// <returns>Created transaction</returns>
    /// <response code="201">Transaction created successfully</response>
    /// <response code="400">Invalid transaction data</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transaction = await _transactionService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = transaction.TransactionId }, transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update transaction
    /// </summary>
    /// <param name="id">Transaction ID</param>
    /// <param name="updateDto">Transaction update data</param>
    /// <returns>Updated transaction</returns>
    /// <response code="200">Transaction updated successfully</response>
    /// <response code="400">Invalid transaction data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Transaction not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateTransactionDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transaction = await _transactionService.UpdateAsync(id, updateDto);
            return Ok(transaction);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Transaction not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete transaction
    /// </summary>
    /// <param name="id">Transaction ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Transaction deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Transaction not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _transactionService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Transaction not found" });
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
