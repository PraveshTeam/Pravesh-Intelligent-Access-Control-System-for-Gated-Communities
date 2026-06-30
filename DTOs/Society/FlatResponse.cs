namespace Pravesh.API.DTOs.Society;

public class FlatResponse
{
    public int Id { get; set; }
    public int SocietyId { get; set; }
    public string FlatNumber { get; set; } = string.Empty;
    public string? Tower { get; set; }
    public int? Floor { get; set; }
    public int? ResidentId { get; set; }
    public string? ResidentName { get; set; }
}