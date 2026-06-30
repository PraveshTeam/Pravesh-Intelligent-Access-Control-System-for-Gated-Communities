using System.ComponentModel.DataAnnotations;

namespace Pravesh.API.DTOs.Society;

public class CreateGateRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Location { get; set; }
}