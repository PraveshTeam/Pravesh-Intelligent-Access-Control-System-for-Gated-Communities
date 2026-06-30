using Pravesh.API.Enums;

namespace Pravesh.API.DTOs.Pass;

public class PassResponse
{
    public int Id { get; set; }
    public string Uuid { get; set; } = string.Empty;
    public int FlatId { get; set; }
    public string FlatNumber { get; set; } = string.Empty;
    public int ResidentId { get; set; }
    public string ResidentName { get; set; } = string.Empty;
    public string VisitorName { get; set; } = string.Empty;
    public string? VisitorPhone { get; set; }
    public string PassType { get; set; } = string.Empty;
    public int UsesAllowed { get; set; }
    public int UsesRemaining { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}