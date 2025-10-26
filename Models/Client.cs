using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("client")]
public partial class Client
{
    [Key]
    [Column("ClientID")]
    public int ClientId { get; set; }

    [StringLength(20)]
    public string? Username { get; set; }

    [Column("FName")]
    [StringLength(20)]
    public string? Fname { get; set; }

    [Column("LName")]
    [StringLength(20)]
    public string? Lname { get; set; }

    [StringLength(30)]
    public string? Email { get; set; }

    public string? CellNum { get; set; }

    [StringLength(25)]
    public string? City { get; set; }

    [StringLength(25)]
    public string? Suburb { get; set; }

    public string? ImageUrl { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public int? PostCode { get; set; }

    [StringLength(25)]
    public string? Password { get; set; }

    [InverseProperty("Client")]
    public virtual ICollection<Puppy> Puppies { get; set; } = new List<Puppy>();

    [InverseProperty("Client")]
    public virtual ICollection<Puppyrequest> Puppyrequests { get; set; } = new List<Puppyrequest>();

    [InverseProperty("Client")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Client")]
    public virtual ICollection<Trainingrequest> Trainingrequests { get; set; } = new List<Trainingrequest>();

    public ICollection<TrainingProgress> TrainingProgresses { get; set; } = new List<TrainingProgress>();
}
