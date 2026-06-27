namespace Pravesh.API.DTOs.Auth;

public class UpdateProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}