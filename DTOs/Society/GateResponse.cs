namespace Pravesh.API.DTOs.Society;

public class GateResponse
{
    public int Id { get; set; }
    public int SocietyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int? AssignedGuardId { get; set; }
    public string? AssignedGuardName { get; set; }
    public bool IsActive { get; set; }
}