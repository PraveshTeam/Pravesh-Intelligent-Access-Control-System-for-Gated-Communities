using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pravesh.API.Entities;

[Table("gates")]
public class Gate
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    // FK → societies.id
    [Required]
    [Column("society_id")]
    public int SocietyId { get; set; }

    [ForeignKey(nameof(SocietyId))]
    public Society Society { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;   // e.g. Main Gate, Rear Gate

    [MaxLength(100)]
    [Column("location")]
    public string? Location { get; set; }

    // FK → users.id (assigned guard)
    [Column("assigned_guard_id")]
    public int? AssignedGuardId { get; set; }

    [ForeignKey(nameof(AssignedGuardId))]
    public User? AssignedGuard { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();
}
