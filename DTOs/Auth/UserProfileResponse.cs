namespace Pravesh.API.DTOs.Auth;

public class UserProfileResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public int? FlatId { get; set; }
    public int? SocietyId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}