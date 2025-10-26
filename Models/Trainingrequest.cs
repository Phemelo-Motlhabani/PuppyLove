using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("trainingrequest")]
[Index("ClientId", Name = "fk_TrainingRequest_Client")]
[Index("TrainerId", Name = "fk_TrainingRequest_Trainer")]
public partial class Trainingrequest
{
    [Key]
    [Column("TRequestID")]
    public int TrequestId { get; set; }

    [Required]
    [Column("ClientID")]
    public int ClientId { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Trainingrequests")]
    public virtual Client Client { get; set; }

    [Required]
    [Column("TrainerID")]
    public int TrainerId { get; set; }

    [ForeignKey("TrainerId")]
    [InverseProperty("Trainingrequests")]
    public virtual Trainer Trainer { get; set; }

    [Required]
    [StringLength(100)]
    public string DogName { get; set; }

    [Required]
    [StringLength(50)]
    public string DogBreed { get; set; }

    public string TrainingProgram { get; set; } 

    [Range(0, 30)]
    public int DogAge { get; set; }

    [StringLength(1000)]
    public string? AdditionalNotes { get; set; }

    [StringLength(1000)]
    public string? AdditionalInfo { get; set; }

    public bool IsAccepted { get; set; } = false;

    [Required]
    public DateTime RequestDate { get; set; } = DateTime.Now;

    [Required]
    [StringLength(20)]
    public string RequestStatus { get; set; } = "Pending"; // Pending, Accepted, Rejected

}
