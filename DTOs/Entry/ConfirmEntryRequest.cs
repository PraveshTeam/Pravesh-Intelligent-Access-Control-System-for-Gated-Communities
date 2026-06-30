using System.ComponentModel.DataAnnotations;

namespace Pravesh.API.DTOs.Entry;

public class ConfirmEntryRequest
{
    [Required]
    public string Uuid { get; set; } = string.Empty;
}