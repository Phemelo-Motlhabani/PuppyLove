using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("parkrecommendation")]
[Index("TrainerId", Name = "fk_ParkRecommendation_Trainer")]
public partial class Parkrecommendation
{
    [Key]
    public int ParkCode { get; set; }

    [StringLength(25)]
    public string? ParkName { get; set; }

    [StringLength(25)]
    public string? City { get; set; }

    [StringLength(25)]
    public string? Suburb { get; set; }
    public string? ImageUrl { get; set; }


    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    [Column("TrainerID")]
    public int TrainerId { get; set; }

    [ForeignKey("TrainerId")]
    [InverseProperty("Parkrecommendations")]
    public virtual Trainer? Trainer { get; set; }
}
