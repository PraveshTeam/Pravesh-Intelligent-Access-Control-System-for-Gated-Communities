namespace Pravesh.API.DTOs.Entry;

public class EntryLogResponse
{
    public int Id { get; set; }
    public int PassId { get; set; }
    public string UuidScanned { get; set; } = string.Empty;
    public string VisitorName { get; set; } = string.Empty;
    public string FlatNumber { get; set; } = string.Empty;
    public int? GateId { get; set; }
    public string? GateName { get; set; }
    public int? GuardId { get; set; }
    public string? GuardName { get; set; }
    public string ScanResult { get; set; } = string.Empty;
    public string? DenyReason { get; set; }
    public DateTime ScannedAt { get; set; }
    public bool ResidentNotified { get; set; }
}