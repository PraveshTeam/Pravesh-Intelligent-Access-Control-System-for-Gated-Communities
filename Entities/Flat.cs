using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pravesh.API.Entities;

[Table("flats")]
public class Flat
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    // FK → societies.id
    [Required]
    [Column("society_id")]
    public int SocietyId { get; set; }

    [ForeignKey(nameof(SocietyId))]
    public Society Society { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    [Column("flat_number")]
    public string FlatNumber { get; set; } = string.Empty;   // e.g. A-304

    [MaxLength(20)]
    [Column("tower")]
    public string? Tower { get; set; }

    [Column("floor")]
    public int? Floor { get; set; }

    // FK → users.id (NULL if flat is vacant)
    [Column("resident_id")]
    public int? ResidentId { get; set; }

    [ForeignKey(nameof(ResidentId))]
    public User? Resident { get; set; }

    // Navigation properties
    public ICollection<VisitorPass> VisitorPasses { get; set; } = new List<VisitorPass>();
}
