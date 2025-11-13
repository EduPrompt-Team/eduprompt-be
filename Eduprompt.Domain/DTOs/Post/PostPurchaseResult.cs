namespace Eduprompt.Domain.DTOs.Post;

public class PostPurchaseResult
{
    public int StorageId { get; set; }
    public int PromptInstanceId { get; set; }
    public string Message { get; set; } = "Purchase completed successfully";
}

