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

    [Required]
    [Column("pass_id")]
    public int PassId { get; set; }

    [ForeignKey(nameof(PassId))]
    public VisitorPass VisitorPass { get; set; } = null!;

    [Required]
    [MaxLength(36)]
    [Column("uuid_scanned")]
    public string UuidScanned { get; set; } = string.Empty;

    [Column("gate_id")]
    public int? GateId { get; set; }

    [ForeignKey(nameof(GateId))]
    public Gate? Gate { get; set; }

    [Column("guard_id")]
    public int? GuardId { get; set; }

    [ForeignKey(nameof(GuardId))]
    public User? Guard { get; set; }

    [Required]
    [Column("scan_result")]
    public ScanResult ScanResult { get; set; }

    [Column("deny_reason")]
    public DenyReason? DenyReason { get; set; }

    [Column("scanned_at")]
    public DateTime ScannedAt { get; set; } = DateTime.Now;

    [Column("resident_notified")]
    public bool ResidentNotified { get; set; } = false;
}