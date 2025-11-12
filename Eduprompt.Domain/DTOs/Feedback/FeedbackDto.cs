namespace Eduprompt.Domain.DTOs.Feedback;

public class FeedbackDto
{
    public int FeedbackId { get; set; }
    public int? PostId { get; set; }
    public int? StorageId { get; set; }
    public int UserId { get; set; }
    public int? PackageId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsVerified { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserProfileUrl { get; set; }
    public string? PostTitle { get; set; }
    public string? StorageTemplateName { get; set; }
}
