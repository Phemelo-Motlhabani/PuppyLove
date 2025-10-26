using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("puppyrequest")]
[Index("BreederId", Name = "fk_PuppyRequest_Breeder")]
[Index("ClientId", Name = "fk_PuppyRequest_Client")]
public partial class Puppyrequest
{
    [Key]
    [Column("RequestID")]
    public int RequestId { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpDate { get; set; }

    [Column("ClientID")]
    public int? ClientId { get; set; }

    [Column("BreederID")]
    public int? BreederId { get; set; }

    [Column("PuppyID")]
    public int PuppyId { get; set; }

    [Column(TypeName = "text")]
    public string? BreederResponse { get; set; }

    [Column(TypeName = "text")]
    public string? Message { get; set; }

    public DateTime RequestDate { get; set; }

    public DateTime? ResponseDate { get; set; }

    [ForeignKey("BreederId")]
    [InverseProperty("Puppyrequests")]
    public virtual Breeder? Breeder { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Puppyrequests")]
    public virtual Client? Client { get; set; }

    [ForeignKey("PuppyId")]
    [InverseProperty("Puppyrequests")]
    public virtual Puppy? Puppy { get; set; }
}
