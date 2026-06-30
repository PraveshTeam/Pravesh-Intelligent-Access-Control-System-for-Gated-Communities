using System.ComponentModel.DataAnnotations;

namespace Pravesh.API.DTOs.Society;

public class CreateSocietyRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }
}