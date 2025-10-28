namespace Eduprompt.Domain.Interface.Service;

public interface ITemplatePurchaseResult
{
    int StorageId { get; }
    int PromptInstanceId { get; }
}

public interface ITemplateCommerceService
{
    Task<ITemplatePurchaseResult> PurchaseTemplateAsync(
        int buyerUserId,
        int templateArchitectureId,
        string mode, // "with_ai" | "direct"
        decimal price,
        CancellationToken cancellationToken = default);
}


