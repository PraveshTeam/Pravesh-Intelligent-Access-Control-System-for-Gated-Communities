using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pravesh.API.Enums;

namespace Pravesh.API.Entities;

[Table("entry_logs")]
public class EntryLog
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    // FK → visitor_passes.id
    [Required]
    [Column("pass_id")]
    public int PassId { get; set; }

    [ForeignKey(nameof(PassId))]
    public VisitorPass VisitorPass { get; set; } = null!;

    // The UUID that was entered/scanned by the guard
    [Required]
    [MaxLength(36)]
    [Column("uuid_scanned")]
    public string UuidScanned { get; set; } = string.Empty;

    // FK → gates.id (nullable — in mini project guard may not always be at a gate)
    [Column("gate_id")]
    public int? GateId { get; set; }

    [ForeignKey(nameof(GateId))]
    public Gate? Gate { get; set; }

    // FK → users.id (the guard who confirmed the entry)
    [Column("guard_id")]
    public int? GuardId { get; set; }

    [ForeignKey(nameof(GuardId))]
    public User? Guard { get; set; }

    [Required]
    [Column("scan_result")]
    public ScanResult ScanResult { get; set; }

    // Populated only when ScanResult == DENIED
    [Column("deny_reason")]
    public DenyReason? DenyReason { get; set; }

    [Column("scanned_at")]
    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

    // Used in Major project; kept here for schema consistency
    [Column("resident_notified")]
    public bool ResidentNotified { get; set; } = false;
}
