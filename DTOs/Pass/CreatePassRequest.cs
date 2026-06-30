using System.ComponentModel.DataAnnotations;
using Pravesh.API.Enums;

namespace Pravesh.API.DTOs.Pass;

public class CreatePassRequest
{
    [Required]
    [MaxLength(100)]
    public string VisitorName { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? VisitorPhone { get; set; }

    [Required]
    public PassType PassType { get; set; } = PassType.ONE_TIME;

    public int UsesAllowed { get; set; } = 1;

    [Required]
    public DateTime ValidFrom { get; set; }

    [Required]
    public DateTime ValidUntil { get; set; }
}