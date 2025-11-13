using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.BLL.Services;

public sealed class TemplatePurchaseResult : ITemplatePurchaseResult
{
    public int StorageId { get; init; }
    public int PromptInstanceId { get; init; }
}

public class TemplateCommerceService : ITemplateCommerceService
{
    private readonly EdupromptV2Context _db;
    private readonly IWalletService _walletService;
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionService _transactionService;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IStorageTemplateRepository _storageTemplateRepository;
    private readonly IPromptInstanceRepository _promptInstanceRepository;
    private readonly IPromptInstanceDetailRepository _promptInstanceDetailRepository;
    private readonly IExpectedOutputService _expectedOutputService;
    private readonly ITemplateArchitectureRepository _templateArchitectureRepository;
    private readonly IOrderRepository _orderRepository;

    public TemplateCommerceService(
        EdupromptV2Context db,
        IWalletService walletService,
        IWalletRepository walletRepository,
        ITransactionService transactionService,
        IPaymentMethodRepository paymentMethodRepository,
        IStorageTemplateRepository storageTemplateRepository,
        IPromptInstanceRepository promptInstanceRepository,
        IPromptInstanceDetailRepository promptInstanceDetailRepository,
        ITemplateArchitectureRepository templateArchitectureRepository,
        IOrderRepository orderRepository,
        IExpectedOutputService expectedOutputService)
    {
        _db = db;
        _walletService = walletService;
        _walletRepository = walletRepository;
        _transactionService = transactionService;
        _paymentMethodRepository = paymentMethodRepository;
        _storageTemplateRepository = storageTemplateRepository;
        _promptInstanceRepository = promptInstanceRepository;
        _promptInstanceDetailRepository = promptInstanceDetailRepository;
        _templateArchitectureRepository = templateArchitectureRepository;
        _orderRepository = orderRepository;
        _expectedOutputService = expectedOutputService;
    }

    public async Task<ITemplatePurchaseResult> PurchaseTemplateAsync(
        int buyerUserId,
        int templateArchitectureId,
        string mode,
        decimal price,
        CancellationToken cancellationToken = default)
    {
        // Load template architecture
        var arch = await _templateArchitectureRepository.GetByIdAsync(templateArchitectureId);
        if (arch == null) throw new InvalidOperationException("Template architecture not found");

        // Best-effort transaction boundary (single DB context)
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

        // Determine seller: from StorageTemplates.UserID if available
        var storage = await _db.StorageTemplates.FirstOrDefaultAsync(s => s.StorageId == arch.StorageId, cancellationToken);
        if (storage == null) throw new InvalidOperationException("Template storage not found for architecture");
        var sellerUserId = storage.UserId;
        if (sellerUserId == buyerUserId) throw new InvalidOperationException("Cannot purchase your own template");

        // 1) Money movement
        // Get wallets for transaction records
        var buyerWallet = await _walletService.GetByUserIdAsync(buyerUserId);
        var sellerWallet = await _walletService.GetByUserIdAsync(sellerUserId);
        
        if (buyerWallet == null)
            throw new InvalidOperationException("Buyer wallet not found");
        if (sellerWallet == null)
            throw new InvalidOperationException("Seller wallet not found");

        // Deduct buyer, credit seller
        await _walletService.DeductFundsByUserIdAsync(buyerUserId, price);
        await _walletService.AddFundsByUserIdAsync(sellerUserId, price);

        // 2) Create order record (template type) - Status = "Paid" because payment is immediate
        var order = new Order
        {
            UserId = buyerUserId,
            PackageId = null,
            TotalAmount = price,
            OrderDate = DateTime.UtcNow,
            Notes = $"Purchase template architecture #{templateArchitectureId}",
            Status = "Paid" // Payment is immediate via wallet, so status = "Paid" (consistent with OrderService)
        };
        order = await _orderRepository.CreateAsync(order);

        // 3) Create transaction records with actual wallet IDs (consistent with PostService)
        try
        {
            // Find Wallet payment method (or use default)
            var paymentMethods = await _paymentMethodRepository.GetAllAsync();
            var walletMethod = paymentMethods.FirstOrDefault(m => 
                (m.MethodName ?? "").Contains("Wallet", StringComparison.OrdinalIgnoreCase) ||
                (m.Provider ?? "").Contains("Wallet", StringComparison.OrdinalIgnoreCase) ||
                (m.Provider ?? "").Contains("Internal", StringComparison.OrdinalIgnoreCase)
            ) ?? paymentMethods.FirstOrDefault();
            
            var paymentMethodId = walletMethod?.PaymentMethodId ?? 1;
            
            // Buyer transaction: Payment (money going out)
            await _transactionService.CreateAsync(new Domain.DTOs.Transaction.CreateTransactionDto
            {
                PaymentMethodId = paymentMethodId,
                WalletId = buyerWallet.WalletId,
                OrderId = order.OrderId,
                Amount = price,
                TransactionType = "Payment", // Consistent with PostService and OrderService
                Status = "Completed", // Wallet payment is completed immediately
                TransactionReference = $"Purchase template architecture #{templateArchitectureId}"
            });
            
            // Seller transaction: Deposit (money coming in)
            await _transactionService.CreateAsync(new Domain.DTOs.Transaction.CreateTransactionDto
            {
                PaymentMethodId = paymentMethodId,
                WalletId = sellerWallet.WalletId,
                OrderId = null, // Seller doesn't have OrderId
                Amount = price,
                TransactionType = "Deposit", // Consistent with PostService
                Status = "Completed", // Wallet payment is completed immediately
                TransactionReference = $"Sale template architecture #{templateArchitectureId}"
            });
        }
        catch (Exception ex)
        {
            // Log error but don't fail the purchase
            // Transaction creation is important but shouldn't block the purchase
            Console.WriteLine($"Warning: Failed to create transaction records for template purchase {templateArchitectureId}: {ex.Message}");
        }

        // 4) Grant ownership: create a StorageTemplate for buyer (copy meta)
        var buyerStorage = new StorageTemplate
        {
            UserId = buyerUserId,
            PackageId = storage.PackageId,
            TemplateName = storage.TemplateName,
            IsFavorite = false,
            CreatedAt = DateTime.UtcNow,
            TemplateContent = storage.TemplateContent,
            Grade = storage.Grade,
            Subject = storage.Subject,
            Chapter = storage.Chapter
        };
        _db.StorageTemplates.Add(buyerStorage);
        await _db.SaveChangesAsync(cancellationToken);

        // 5) Clone PromptInstance to buyer (simple instance with name)
        var newInstance = new PromptInstance
        {
            UserId = buyerUserId,
            PackageId = storage.PackageId,
            PromptName = storage.TemplateName ?? $"Template-{templateArchitectureId}",
            InputJson = null,
            OutputJson = null,
            ExecutedAt = DateTime.UtcNow,
            ProcessingTimeMs = null,
            Status = "Completed"
        };
        newInstance = await _promptInstanceRepository.CreateAsync(newInstance);

        // 4b) Clone PromptInstanceDetails if any prototype exists for this architecture's storage
        var prototypeInstance = await _db.PromptInstances
            .Include(i => i.PromptInstanceDetails)
            .Where(i => i.UserId == sellerUserId && i.PromptName == storage.TemplateName)
            .OrderByDescending(i => i.InstanceId)
            .FirstOrDefaultAsync(cancellationToken);
        if (prototypeInstance?.PromptInstanceDetails != null)
        {
            foreach (var d in prototypeInstance.PromptInstanceDetails)
            {
                var clone = new PromptInstanceDetail
                {
                    InstanceId = newInstance.InstanceId,
                    ParameterName = d.ParameterName,
                    ParameterValue = d.ParameterValue,
                    ParameterType = d.ParameterType
                };
                await _promptInstanceDetailRepository.CreateAsync(clone);
            }
        }

        // optional ExpectedOutput generation for with_ai
        if (string.Equals(mode, "with_ai", StringComparison.OrdinalIgnoreCase))
        {
            await _expectedOutputService.CreateAsync(new Domain.DTOs.ExpectedOutput.CreateExpectedOutputDto
            {
                PromptInstanceId = newInstance.InstanceId,
                OutputName = $"Auto-Generated for {newInstance.PromptName}",
                Status = "Active",
                OutputDetails = null
            });
        }

        await tx.CommitAsync(cancellationToken);

        return new TemplatePurchaseResult
        {
            StorageId = buyerStorage.StorageId,
            PromptInstanceId = newInstance.InstanceId
        };
    }
}


