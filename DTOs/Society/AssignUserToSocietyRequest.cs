using System.ComponentModel.DataAnnotations;

namespace Pravesh.API.DTOs.Society;

public class AssignUserToSocietyRequest
{
    [Required]
    public int UserId { get; set; }
}