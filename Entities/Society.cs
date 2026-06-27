using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pravesh.API.Entities;

[Table("societies")]
public class Society
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("address", TypeName = "TEXT")]
    public string? Address { get; set; }

    [MaxLength(100)]
    [Column("city")]
    public string? City { get; set; }

    // FK → users.id (Society Admin)
    [Column("admin_id")]
    public int? AdminId { get; set; }

    [ForeignKey(nameof(AdminId))]
    public User? Admin { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Flat> Flats { get; set; } = new List<Flat>();
    public ICollection<Gate> Gates { get; set; } = new List<Gate>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
