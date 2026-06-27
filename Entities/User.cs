using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pravesh.API.Enums;

namespace Pravesh.API.Entities;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(15)]
    [Column("phone")]
    public string? Phone { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("role")]
    public UserRole Role { get; set; }

    // FK → flats.id (null if user is not a resident)
    [Column("flat_id")]
    public int? FlatId { get; set; }

    [ForeignKey(nameof(FlatId))]
    public Flat? Flat { get; set; }

    // FK → societies.id
    [Column("society_id")]
    public int? SocietyId { get; set; }

    [ForeignKey(nameof(SocietyId))]
    public Society? Society { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<VisitorPass> VisitorPasses { get; set; } = new List<VisitorPass>();
    public ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();
}
