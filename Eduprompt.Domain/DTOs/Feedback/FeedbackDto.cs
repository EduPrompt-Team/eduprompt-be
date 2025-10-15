namespace Eduprompt.Domain.DTOs.Feedback;

public class FeedbackDto
{
    public int FeedbackID { get; set; }
    public int PostID { get; set; }
    public int UserID { get; set; }
    public int? PackageID { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsVerified { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? PostTitle { get; set; }
}
