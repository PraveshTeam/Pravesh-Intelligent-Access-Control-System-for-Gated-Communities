using System.ComponentModel.DataAnnotations;

namespace Pravesh.API.DTOs.Society;

public class AssignSocietyAdminRequest
{
    [Required]
    public int UserId { get; set; }
}