using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("puppy")]
[Index("ClientId", Name = "fk_Puppy_Client")]
[Index("LitterId", Name = "fk_Puppy_Litter")]
public partial class Puppy
{
    [Key]
    [Column("PuppyID")]
    public int PuppyId { get; set; }

    [Column("LitterID")]
    public int? LitterId { get; set; }

    [Column("ClientID")]
    public int? ClientId { get; set; }

    [StringLength(25)]
    public string? PuppyName { get; set; }

    public int? Age { get; set; }

    [StringLength(1)]
    public string? TrainingStatus { get; set; }

    [StringLength(10)]
    public string? Colour { get; set; }

    [StringLength(1)]
    public string? Gender { get; set; }

    [StringLength(2)]
    public string? Size { get; set; }

    public double? Price { get; set; }

    [StringLength(1)]
    public string? SaleStatus { get; set; }
    public string? ImageUrl { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Puppies")]
    public virtual Client? Client { get; set; }

    [ForeignKey("LitterId")]
    [InverseProperty("Puppies")]
    public virtual Litter? Litter { get; set; }
}
