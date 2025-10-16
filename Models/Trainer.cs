using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("trainer")]
public partial class Trainer
{
    [Key]
    [Column("TrainerID")]
    public int TrainerId { get; set; } 

    [StringLength(25)]
    public string? Username { get; set; }

    [Column("FName")]
    [StringLength(25)]
    public string? Fname { get; set; }

    [Column("LName")]
    [StringLength(25)]
    public string? Lname { get; set; }

    [StringLength(30)]
    public string? Email { get; set; }

    public string? CellNum { get; set; }

    [StringLength(25)]
    public string? City { get; set; }

    [StringLength(25)]
    public string? Suburb { get; set; }

    [StringLength(25)]
    public string? Password { get; set; }

    [StringLength(1)]
    public string? VerificationStatus { get; set; }

    public string? ImageUrl { get; set; }

    [NotMapped]
    public IFormFile? ImageFile {  get; set; } 

    [InverseProperty("Trainer")]
    public virtual ICollection<Parkrecommendation> Parkrecommendations { get; set; } = new List<Parkrecommendation>();

    [InverseProperty("Trainer")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Trainer")]
    public virtual ICollection<Trainerskill> Trainerskills { get; set; } = new List<Trainerskill>();

    [InverseProperty("Trainer")]
    public virtual ICollection<Trainingrequest> Trainingrequests { get; set; } = new List<Trainingrequest>();

    /*[InverseProperty("Trequest")]
    public virtual Trainingrequest? Trainingrequest { get; set; }*/
    //[InverseProperty("Trainer")]
    public ICollection<TrainingProgress> TrainingProgresses { get; set; } = new List<TrainingProgress>();

    [ForeignKey("TrainerId")]
    [InverseProperty("Trainers")]
    public virtual ICollection<Breedtype> Breeds { get; set; } = new List<Breedtype>();
}
