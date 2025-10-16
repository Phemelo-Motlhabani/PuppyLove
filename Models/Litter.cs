using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("litter")]
[Index("BreedId", Name = "fk_Litter_Breed")]
[Index("BreederId", Name = "fk_Litter_Breeder")]
public partial class Litter
{
    [Key]
    [Column("LitterID")]
    public int LitterId { get; set; }

    [Column("BreedID")]
    public int? BreedId { get; set; }

    [Column("BreederID")]
    public int? BreederId { get; set; }

    public int? NumPuppies { get; set; }

    [ForeignKey("BreedId")]
    [InverseProperty("Litters")]
    public virtual Breedtype? BreedType { get; set; }

    [ForeignKey("BreederId")]
    [InverseProperty("Litters")]
    public virtual Breeder? Breeder { get; set; }

    [InverseProperty("Litter")]
    public virtual ICollection<Puppy> Puppies { get; set; } = new List<Puppy>();
}
