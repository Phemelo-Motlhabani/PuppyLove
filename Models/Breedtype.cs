using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("breedtype")]
public partial class Breedtype
{
    [Key]
    [Column("BreedID")]
    public int BreedId { get; set; }

    [StringLength(20)]
    public string? BreedName { get; set; }

    [StringLength(2)]
    public string? Size { get; set; }

    [StringLength(1)]
    public string? ActivityLevel { get; set; }

    [StringLength(1)]
    public string? Grooming { get; set; }

    public int? SaleCount { get; set; }

    [InverseProperty("Breed")]
    public virtual ICollection<Litter> Litters { get; set; } = new List<Litter>();

    [ForeignKey("BreedId")]
    [InverseProperty("Breeds")]
    public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
}
