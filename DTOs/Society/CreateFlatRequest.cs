using System.ComponentModel.DataAnnotations;

namespace Pravesh.API.DTOs.Society;

public class CreateFlatRequest
{
    [Required]
    [MaxLength(20)]
    public string FlatNumber { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Tower { get; set; }

    public int? Floor { get; set; }
}