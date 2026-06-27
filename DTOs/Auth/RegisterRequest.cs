using System.ComponentModel.DataAnnotations;
using Pravesh.API.Enums;

namespace Pravesh.API.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? Phone { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.RESIDENT;

    public int? SocietyId { get; set; }
    public int? FlatId { get; set; }
}