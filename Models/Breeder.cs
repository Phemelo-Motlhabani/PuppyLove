using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("breeder")]
public partial class Breeder
{
    [Key]
    [Column("BreederID")]
    public int BreederId { get; set; }

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

    [StringLength(25)]
    public string? Password { get; set; }

    [StringLength(25)]
    public string? KennelName { get; set; }

    public long? LicenceNum { get; set; }

    public string? ImageUrl { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    [StringLength(1)]
    public string? VerificationStatus { get; set; }

    [InverseProperty("Breeder")]
    public virtual ICollection<Litter> Litters { get; set; } = new List<Litter>();

    [InverseProperty("Breeder")]
    public virtual ICollection<Puppyrequest> Puppyrequests { get; set; } = new List<Puppyrequest>();

    [InverseProperty("Breeder")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
