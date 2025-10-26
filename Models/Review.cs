using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("review")]
[Index("BreederId", Name = "fk_Review_Breeder")]
[Index("ClientId", Name = "fk_Review_Client")]
[Index("TrainerId", Name = "fk_Review_Trainer")]
public partial class Review
{
    [Key]
    [Column("ReviewID")]
    public int ReviewId { get; set; }
    [Required]
    [Range(1,5, ErrorMessage ="Ratin gmust be between 1 and 5")]
    public double Rating { get; set; }

    [StringLength(255)]
    public string? ReviewText { get; set; }

    public DateTime ReviewDate { get; set; }

    [Column("ClientID")]
    public int? ClientId { get; set; }

    [Column("BreederID")]
    public int? BreederId { get; set; }

    [Column("TrainerID")]
    public int? TrainerId { get; set; }

    [ForeignKey("BreederId")]
    [InverseProperty("Reviews")]
    public virtual Breeder? Breeder { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Reviews")]
    public virtual Client? Client { get; set; }

    [ForeignKey("TrainerId")]
    [InverseProperty("Reviews")]
    public virtual Trainer? Trainer { get; set; }
}
