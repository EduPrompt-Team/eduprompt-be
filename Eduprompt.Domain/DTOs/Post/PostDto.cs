namespace Eduprompt.Domain.DTOs.Post;

public class PostDto
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? PostType { get; set; }
    public string? Tags { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public double AverageRating { get; set; }
    public int FeedbackCount { get; set; }
}
