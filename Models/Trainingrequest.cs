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

    [Column("ClientID")]
    public int ClientId { get; set; }

    [Column("TrainerID")]
    public int TrainerId { get; set; }

    [StringLength(100)]
    public string? TrainingProgram { get; set; }

    [StringLength(20)]
    public string RequestStatus { get; set; } = "Pending";

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    public bool IsAccepted { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Trainingrequests")]
    public virtual Client Client { get; set; }

    [ForeignKey("TrainerId")]
    [InverseProperty("Trainingrequests")]
    public virtual Trainer Trainer { get; set; }
}
