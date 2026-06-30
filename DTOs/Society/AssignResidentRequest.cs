using System.ComponentModel.DataAnnotations;

namespace Pravesh.API.DTOs.Society;

public class AssignResidentRequest
{
    [Required]
    public int ResidentId { get; set; }
}