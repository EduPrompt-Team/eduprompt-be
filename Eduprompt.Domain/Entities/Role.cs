using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Roles")]
public partial class Role
{
    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [Required]
    [StringLength(50)]
    [Column("RoleName")]
    public string RoleName { get; set; } = string.Empty;

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}