using System.ComponentModel.DataAnnotations;

namespace Pravesh.API.DTOs.Society;

public class AssignGuardRequest
{
    [Required]
    public int GuardId { get; set; }
}