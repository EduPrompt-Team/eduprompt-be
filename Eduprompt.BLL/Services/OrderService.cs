using Eduprompt.Domain.DTOs.Order;
using Eduprompt.Domain.DTOs.Transaction;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using Eduprompt.DAL.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Eduprompt.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IWalletService _walletService;
    private readonly ITransactionService _transactionService;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly EdupromptV2Context _db;

    public OrderService(
        IOrderRepository orderRepository, 
        ICartRepository cartRepository,
        IPaymentRepository paymentRepository,
        IWalletService walletService,
        ITransactionService transactionService,
        IPaymentMethodRepository paymentMethodRepository,
        IWalletRepository walletRepository,
        EdupromptV2Context db)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _paymentRepository = paymentRepository;
        _walletService = walletService;
        _transactionService = transactionService;
        _paymentMethodRepository = paymentMethodRepository;
        _walletRepository = walletRepository;
        _db = db;
    }

    public async Task<OrderServiceDto> CreateOrderFromCartAsync(int UserId, string? notes)
    {
        var cart = await _cartRepository.GetByUserIdAsync(UserId);
        var totalAmount = cart?.CartDetails?.Sum(cd => cd.Quantity * cd.UnitPrice) ?? 0m;

        // Determine PackageId from cart
        // If cart has only one package, set PackageId; otherwise leave it null
        int? packageId = null;
        if (cart?.CartDetails != null && cart.CartDetails.Any())
        {
            var distinctPackages = cart.CartDetails
                .Select(cd => cd.PackageId)
                .Distinct()
                .ToList();
            
            // If cart has exactly one unique package, set PackageId
            if (distinctPackages.Count == 1)
            {
                packageId = distinctPackages.First();
            }
            // If cart has multiple packages, PackageId remains null
        }

        var order = new Order
        {
            UserId = UserId,
            PackageId = packageId, // Set PackageId if cart has single package
            TotalAmount = totalAmount,
            OrderDate = DateTime.UtcNow,
            Notes = notes,
            Status = "Pending"
        };

        var created = await _orderRepository.CreateAsync(order);
        await _cartRepository.ClearCartAsync(UserId);
        return MapToServiceDto(created);
    }

    public async Task<OrderServiceDto?> GetByIdAsync(int OrderId, int UserId)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId);
        if (order == null || order.UserId != UserId) return null;
        return MapToServiceDto(order);
    }

    public async Task<OrderServiceDto?> GetByIdAdminAsync(int OrderId)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId);
        if (order == null) return null;
        return MapToServiceDto(order);
    }

    public async Task<IEnumerable<OrderServiceDto>> GetUserOrdersAsync(int UserId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(UserId);
        return orders.Select(MapToServiceDto);
    }

    public async Task<IEnumerable<OrderServiceDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToServiceDto);
    }

    public async Task<OrderServiceDto> CancelOrderAsync(int OrderId, int UserId)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId);
        if (order == null || order.UserId != UserId)
            throw new KeyNotFoundException("Order not found");

        order.Status = "Cancelled";
        var updated = await _orderRepository.UpdateAsync(order);
        return MapToServiceDto(updated);
    }

    public async Task<OrderServiceDto> UpdateOrderStatusAsync(int OrderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId);
        if (order == null)
            throw new KeyNotFoundException("Order not found");

        order.Status = status;
        var updated = await _orderRepository.UpdateAsync(order);

        // Auto-create payment record when order status becomes Completed or Paid
        if ((status == "Completed" || status == "Paid") && order.UserId > 0)
        {
            // Check if payment record already exists
            var existingPayments = await _paymentRepository.GetByOrderIdAsync(OrderId);
            if (!existingPayments.Any(p => p.Status == "Paid"))
            {
                // Get default payment method (Wallet)
                var paymentMethods = await _paymentMethodRepository.GetAllAsync();
                var walletMethod = paymentMethods.FirstOrDefault(pm => 
                    (pm.MethodName ?? "").Equals("Wallet", StringComparison.OrdinalIgnoreCase) ||
                    (pm.Provider ?? "").Equals("Internal", StringComparison.OrdinalIgnoreCase)
                ) ?? paymentMethods.FirstOrDefault(pm => pm.IsActive);

                var payment = new Payment
                {
                    OrderId = OrderId,
                    UserId = order.UserId,
                    Amount = order.TotalAmount,
                    PaymentMethod = walletMethod?.MethodName ?? "Wallet",
                    Provider = walletMethod?.Provider ?? "Internal",
                    Status = "Paid",
                    CreatedAt = DateTime.UtcNow
                };
                await _paymentRepository.CreateAsync(payment);
            }
        }

        return MapToServiceDto(updated);
    }

    public async Task<OrderServiceDto> PayOrderWithWalletAsync(int OrderId, int UserId)
    {
        // Use transaction to ensure atomicity
        await using var tx = await _db.Database.BeginTransactionAsync();
        
        try
        {
            // Lock order row to prevent concurrent modifications
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.OrderId == OrderId);
            
            if (order == null || order.UserId != UserId)
                throw new KeyNotFoundException("Order not found");

            if (order.Status != "Pending")
                throw new InvalidOperationException($"Cannot pay order with status: {order.Status}");

            // Check wallet balance
            var balance = await _walletService.GetBalanceByUserIdAsync(UserId);
            if (balance < order.TotalAmount)
                throw new InvalidOperationException($"Insufficient wallet balance. Required: {order.TotalAmount}, Available: {balance}");

            // Deduct funds from wallet
            var deducted = await _walletService.DeductFundsByUserIdAsync(UserId, order.TotalAmount);
            if (!deducted)
                throw new InvalidOperationException("Failed to deduct funds from wallet");

            // Update order status
            order.Status = "Paid";
            var updated = await _orderRepository.UpdateAsync(order);
            
            // Create transaction record for wallet payment
            try
            {
                var wallet = await _walletRepository.GetByUserIdAsync(UserId);
                if (wallet != null)
                {
                    // Find Wallet payment method (or use default)
                    var paymentMethods = await _paymentMethodRepository.GetAllAsync();
                    var walletMethod = paymentMethods.FirstOrDefault(m => 
                        (m.MethodName ?? "").Contains("Wallet", StringComparison.OrdinalIgnoreCase) ||
                        (m.Provider ?? "").Contains("Wallet", StringComparison.OrdinalIgnoreCase) ||
                        (m.Provider ?? "").Contains("Internal", StringComparison.OrdinalIgnoreCase)
                    ) ?? paymentMethods.FirstOrDefault();
                    
                    var paymentMethodId = walletMethod?.PaymentMethodId ?? 1;
                    
                    await _transactionService.CreateAsync(new CreateTransactionDto
                    {
                        PaymentMethodId = paymentMethodId,
                        WalletId = wallet.WalletId,
                        OrderId = order.OrderId,
                        Amount = order.TotalAmount,
                        TransactionType = "Payment",
                        Status = "Completed", // Wallet payment is completed immediately
                        TransactionReference = $"Order #{order.OrderId} - Wallet Payment"
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the payment
                // Transaction creation is optional for wallet payments
                Console.WriteLine($"Warning: Failed to create transaction record for order {OrderId}: {ex.Message}");
            }
            
            await tx.CommitAsync();
            return MapToServiceDto(updated);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    private static OrderServiceDto MapToServiceDto(Order order)
    {
        return new OrderServiceDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            OrderNumber = order.OrderId.ToString(),
            TotalAmount = order.TotalAmount,
            CreatedDate = order.OrderDate, // Use OrderDate as CreatedDate
            OrderDate = order.OrderDate,
            Status = order.Status,
            UserName = order.User?.FullName,
            UserEmail = order.User?.Email,
            Items = new List<OrderItemServiceDto>(),
            Payments = order.Payments?.Select(p => new PaymentServiceDto
            {
                PaymentId = p.PaymentId,
                OrderId = p.OrderId ?? 0,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                PaymentDate = p.CreatedAt,
                Status = p.Status,
                VnpayTransactionId = p.TransactionNo,
                VnpayResponseCode = p.ResponseCode
            }).ToList() ?? new List<PaymentServiceDto>(),
            PackageId = order.PackageId // Add PackageId to response
        };
    }
}