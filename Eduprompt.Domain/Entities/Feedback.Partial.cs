namespace Eduprompt.Domain.Entities;

// Partial extension to support StorageTemplate feedbacks
public partial class Feedback
{
    // Note: PostId in base class is int, but database allows NULL
    // EF Core will handle null values correctly based on DbContext mapping
    
    public int? StorageId { get; set; }
    
    public virtual StorageTemplate? StorageTemplate { get; set; }
}

