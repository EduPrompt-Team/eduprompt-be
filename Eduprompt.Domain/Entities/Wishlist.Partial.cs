namespace Eduprompt.Domain.Entities;

public partial class Wishlist
{
    // Add StorageId for linking to StorageTemplates (prompt templates)
    public int? StorageId { get; set; }
    
    // Navigation property to StorageTemplate
    public virtual StorageTemplate? StorageTemplate { get; set; }
}

