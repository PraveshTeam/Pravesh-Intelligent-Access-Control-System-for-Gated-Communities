namespace Pravesh.API.DTOs.Society;

public class SocietyResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public int? AdminId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}