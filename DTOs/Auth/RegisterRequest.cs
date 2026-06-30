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
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string Password { get; set; } = string.Empty;

    public string? Phone { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.RESIDENT;
}