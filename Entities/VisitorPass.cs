using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pravesh.API.Enums;

namespace Pravesh.API.Entities;

[Table("visitor_passes")]
public class VisitorPass
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    // Unique UUID — used as the pass identifier for entry confirmation (QR payload in Major)
    [Required]
    [MaxLength(36)]
    [Column("uuid")]
    public string Uuid { get; set; } = Guid.NewGuid().ToString();

    // FK → flats.id
    [Required]
    [Column("flat_id")]
    public int FlatId { get; set; }

    [ForeignKey(nameof(FlatId))]
    public Flat Flat { get; set; } = null!;

    // FK → users.id (resident who created the pass)
    [Required]
    [Column("resident_id")]
    public int ResidentId { get; set; }

    [ForeignKey(nameof(ResidentId))]
    public User Resident { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [Column("visitor_name")]
    public string VisitorName { get; set; } = string.Empty;

    [MaxLength(15)]
    [Column("visitor_phone")]
    public string? VisitorPhone { get; set; }

    [Required]
    [Column("pass_type")]
    public PassType PassType { get; set; } = PassType.ONE_TIME;

    // For MULTI_USE passes — total allowed uses
    [Column("uses_allowed")]
    public int UsesAllowed { get; set; } = 1;

    // Decrements on each successful entry confirmation
    [Column("uses_remaining")]
    public int UsesRemaining { get; set; } = 1;

    [Required]
    [Column("valid_from")]
    public DateTime ValidFrom { get; set; }

    [Required]
    [Column("valid_until")]
    public DateTime ValidUntil { get; set; }

    [Required]
    [Column("status")]
    public PassStatus Status { get; set; } = PassStatus.ACTIVE;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<EntryLog> EntryLogs { get; set; } = new List<EntryLog>();
}
