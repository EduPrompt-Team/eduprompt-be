using Eduprompt.Domain.DTOs.Post;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using Eduprompt.DAL.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.BLL.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IStorageTemplateRepository _storageTemplateRepository;
    private readonly IWalletService _walletService;
    private readonly ITransactionService _transactionService;
    private readonly IPromptInstanceRepository _promptInstanceRepository;
    private readonly EdupromptV2Context _db;

    public PostService(
        IPostRepository postRepository,
        IStorageTemplateRepository storageTemplateRepository,
        IWalletService walletService,
        ITransactionService transactionService,
        IPromptInstanceRepository promptInstanceRepository,
        EdupromptV2Context db)
    {
        _postRepository = postRepository;
        _storageTemplateRepository = storageTemplateRepository;
        _walletService = walletService;
        _transactionService = transactionService;
        _promptInstanceRepository = promptInstanceRepository;
        _db = db;
    }

    public async Task<PostDto?> GetByIdAsync(int PostId)
    {
        var post = await _postRepository.GetByIdAsync(PostId);
        if (post == null) return null;

        var dto = MapToDto(post);
        dto.AverageRating = await GetAverageRatingAsync(PostId);
        dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
        return dto;
    }

    public async Task<IEnumerable<PostDto>> GetAllAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        var result = new List<PostDto>();
        foreach (var post in posts)
        {
            var dto = MapToDto(post);
            dto.AverageRating = await GetAverageRatingAsync(post.PostId);
            dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
            result.Add(dto);
        }
        return result;
    }

    public async Task<IEnumerable<PostDto>> GetPublishedPostsAsync()
    {
        var posts = await _postRepository.GetPublishedPostsAsync();
        var result = new List<PostDto>();
        foreach (var post in posts)
        {
            var dto = MapToDto(post);
            dto.AverageRating = await GetAverageRatingAsync(post.PostId);
            dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
            result.Add(dto);
        }
        return result;
    }

    public async Task<IEnumerable<PostDto>> GetByPostTypeAsync(string postType)
    {
        var posts = await _postRepository.GetByPostTypeAsync(postType);
        var result = new List<PostDto>();
        foreach (var post in posts)
        {
            var dto = MapToDto(post);
            dto.AverageRating = await GetAverageRatingAsync(post.PostId);
            dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
            result.Add(dto);
        }
        return result;
    }

    public async Task<PostDto> CreateAsync(CreatePostDto createPostDto)
    {
        var post = new Post
        {
            UserId = createPostDto.UserId,
            Title = createPostDto.Title,
            Content = createPostDto.Content,
            Status = createPostDto.Status ?? "Published",
            PostType = createPostDto.PostType ?? "General",
            Tags = createPostDto.Tags,
            PublishedAt = DateTime.UtcNow
        };
        // Attach template link if provided
        if (createPostDto.TemplateArchitectureId.HasValue)
        {
            typeof(Post).GetProperty("TemplateArchitectureId")?.SetValue(post, createPostDto.TemplateArchitectureId.Value);
        }
        // Attach StorageId and Price if provided
        if (createPostDto.StorageId.HasValue)
        {
            typeof(Post).GetProperty("StorageId")?.SetValue(post, createPostDto.StorageId.Value);
        }
        if (createPostDto.Price.HasValue)
        {
            typeof(Post).GetProperty("Price")?.SetValue(post, createPostDto.Price.Value);
        }

        var createdPost = await _postRepository.CreateAsync(post);
        return MapToDto(createdPost);
    }

    public async Task<PostDto> UpdateAsync(int PostId, CreatePostDto updateDto)
    {
        var post = await _postRepository.GetByIdAsync(PostId);
        if (post == null) throw new KeyNotFoundException("Post not found");

        post.Title = updateDto.Title;
        post.Content = updateDto.Content;
        post.Status = updateDto.Status ?? post.Status;
        post.PostType = updateDto.PostType ?? post.PostType;
        post.Tags = updateDto.Tags ?? post.Tags;
        if (updateDto.TemplateArchitectureId.HasValue)
        {
            typeof(Post).GetProperty("TemplateArchitectureId")?.SetValue(post, updateDto.TemplateArchitectureId.Value);
        }
        if (updateDto.StorageId.HasValue)
        {
            typeof(Post).GetProperty("StorageId")?.SetValue(post, updateDto.StorageId.Value);
        }
        if (updateDto.Price.HasValue)
        {
            typeof(Post).GetProperty("Price")?.SetValue(post, updateDto.Price.Value);
        }

        var updatedPost = await _postRepository.UpdateAsync(post);
        return MapToDto(updatedPost);
    }

    public async Task<IEnumerable<PostDto>> SearchAsync(string searchTerm)
    {
        var posts = await _postRepository.GetAllAsync();
        var lowerSearch = searchTerm.ToLower();
        return posts.Where(p => 
            (p.Title != null && p.Title.ToLower().Contains(lowerSearch)) || 
            (p.Content != null && p.Content.ToLower().Contains(lowerSearch))
        ).Select(MapToDto);
    }

    public async Task<bool> IncrementViewCountAsync(int PostId)
    {
        return await _postRepository.IncrementViewCountAsync(PostId);
    }

    public async Task<bool> IncrementLikeCountAsync(int PostId)
    {
        return await _postRepository.IncrementLikeCountAsync(PostId);
    }

    public async Task<double> GetAverageRatingAsync(int PostId)
    {
        var post = await _postRepository.GetByIdAsync(PostId);
        if (post?.Feedbacks == null || !post.Feedbacks.Any()) return 0.0;
        return post.Feedbacks.Average(f => f.Rating);
    }

    private static PostDto MapToDto(Post post)
    {
        var storageId = typeof(Post).GetProperty("StorageId")?.GetValue(post) as int?;
        var price = typeof(Post).GetProperty("Price")?.GetValue(post) as decimal?;
        
        return new PostDto
        {
            PostId = post.PostId,
            UserId = post.UserId,
            Title = post.Title,
            Content = post.Content,
            PostType = post.PostType,
            Tags = post.Tags,
            ViewCount = post.ViewCount,
            LikeCount = post.LikeCount,
            CreatedDate = post.PublishedAt,
            Status = post.Status,
            UserName = post.User?.FullName ?? "Unknown User",
            AverageRating = 0.0,
            FeedbackCount = post.Feedbacks?.Count ?? 0,
            StorageId = storageId,
            Price = price
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _postRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PostDto>> GetByUserIdAsync(int UserId)
    {
        var posts = await _postRepository.GetByUserIdAsync(UserId);
        var result = new List<PostDto>();
        foreach (var post in posts)
        {
            var dto = MapToDto(post);
            dto.AverageRating = await GetAverageRatingAsync(post.PostId);
            dto.FeedbackCount = post.Feedbacks?.Count ?? 0;
            result.Add(dto);
        }
        return result;
    }

    public async Task<PostPurchaseResult> PurchasePostAsync(int postId, int buyerUserId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post not found");

        // Check if post is for sale
        var storageId = typeof(Post).GetProperty("StorageId")?.GetValue(post) as int?;
        if (!storageId.HasValue)
            throw new InvalidOperationException("Post does not have a linked template for sale");

        var price = typeof(Post).GetProperty("Price")?.GetValue(post) as decimal? ?? 0;
        var sellerUserId = post.UserId;

        if (sellerUserId == buyerUserId)
            throw new InvalidOperationException("Cannot purchase your own template");

        // Check post status
        if (post.Status == "Sold")
            throw new InvalidOperationException("Template has already been sold");

        // Get storage template
        var storage = await _storageTemplateRepository.GetByIdAsync(storageId.Value);
        if (storage == null)
            throw new KeyNotFoundException("Storage template not found");

        // Start transaction with isolation level to prevent concurrent purchases
        await using var tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        try
        {
            // Lock post row to prevent concurrent purchases (race condition)
            var lockedPost = await _db.Posts
                .FirstOrDefaultAsync(p => p.PostId == postId);
            
            if (lockedPost == null)
                throw new KeyNotFoundException("Post not found");
            
            if (lockedPost.Status == "Sold")
                throw new InvalidOperationException("Template has already been sold");

            // 1. Money movement
            if (price > 0)
            {
                // Check buyer balance
                var buyerBalance = await _walletService.GetBalanceByUserIdAsync(buyerUserId);
                if (buyerBalance < price)
                    throw new InvalidOperationException($"Insufficient balance. Required: {price}, Available: {buyerBalance}");

                // Get wallets for transaction records
                var buyerWallet = await _walletService.GetByUserIdAsync(buyerUserId);
                var sellerWallet = await _walletService.GetByUserIdAsync(sellerUserId);
                
                if (buyerWallet == null)
                    throw new InvalidOperationException("Buyer wallet not found");
                if (sellerWallet == null)
                    throw new InvalidOperationException("Seller wallet not found");

                // Deduct from buyer, add to seller
                await _walletService.DeductFundsByUserIdAsync(buyerUserId, price);
                await _walletService.AddFundsByUserIdAsync(sellerUserId, price);

                // Create transaction records with actual wallet IDs
                await _transactionService.CreateAsync(new Domain.DTOs.Transaction.CreateTransactionDto
                {
                    PaymentMethodId = 1, // Default to Wallet payment method (adjust if needed)
                    WalletId = buyerWallet.WalletId,
                    OrderId = null,
                    Amount = price,
                    TransactionType = "Payment",
                    Status = "Completed",
                    TransactionReference = $"Purchase template from post #{postId}"
                });
                await _transactionService.CreateAsync(new Domain.DTOs.Transaction.CreateTransactionDto
                {
                    PaymentMethodId = 1, // Default to Wallet payment method (adjust if needed)
                    WalletId = sellerWallet.WalletId,
                    OrderId = null,
                    Amount = price,
                    TransactionType = "Deposit",
                    Status = "Completed",
                    TransactionReference = $"Sale template from post #{postId}"
                });
            }

            // 2. Create StorageTemplate for buyer (copy from seller)
            var buyerStorage = new StorageTemplate
            {
                UserId = buyerUserId,
                PackageId = storage.PackageId,
                TemplateName = storage.TemplateName,
                TemplateContent = storage.TemplateContent,
                Grade = storage.Grade,
                Subject = storage.Subject,
                Chapter = storage.Chapter,
                IsPublic = false,
                IsFavorite = false,
                CreatedAt = DateTime.UtcNow
            };
            buyerStorage = await _storageTemplateRepository.CreateAsync(buyerStorage);

            // 3. Create PromptInstance for buyer
            var buyerInstance = new PromptInstance
            {
                UserId = buyerUserId,
                PackageId = storage.PackageId,
                PromptName = storage.TemplateName ?? $"Template from Post #{postId}",
                InputJson = null,
                OutputJson = null,
                ExecutedAt = DateTime.UtcNow,
                Status = "Completed"
            };
            buyerInstance = await _promptInstanceRepository.CreateAsync(buyerInstance);

            // 4. Update post status to "Sold" (use locked post)
            lockedPost.Status = "Sold";
            await _postRepository.UpdateAsync(lockedPost);

            await tx.CommitAsync();

            return new PostPurchaseResult
            {
                StorageId = buyerStorage.StorageId,
                PromptInstanceId = buyerInstance.InstanceId,
                Message = "Purchase completed successfully"
            };
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}












