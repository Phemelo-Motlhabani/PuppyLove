using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("puppy")]
[Index("ClientId", Name = "fk_Puppy_Client")]
[Index("LitterId", Name = "fk_Puppy_Litter")]
public partial class Puppy
{
    [Key]
    [Column("PuppyID")]
    public int PuppyId { get; set; }

    [Column("LitterID")]
    public int? LitterId { get; set; }

    [Column("ClientID")]
    public int? ClientId { get; set; }

    [Column("BreederID")]
    public int? BreederId { get; set; }

    [StringLength(25)]
    public string? PuppyName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public int? Age { get; set; }

    [StringLength(20)]
    public string? TrainingStatus { get; set; }

    [StringLength(50)]
    public string? Colour { get; set; }
    public decimal? Weight { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [StringLength(20)]
    public string? Size { get; set; }

    [StringLength(100)]
    public string? Description { get; set; }

    public decimal? Price { get; set; }
    public string ? Status { get; set; }


    [StringLength(20)]
    public string? SaleStatus { get; set; }
    [StringLength(50)]
    public string? HealthStatus { get; set; }
    [StringLength(20)]
    [Display(Name = "Vaccinated")]
    public string? Vaccinated { get; set; } 

    [Display(Name = "Vaccination Date")]
    [DataType(DataType.Date)]
    public DateTime? VaccinationDate { get; set; }

    [StringLength(50)]
    [Display(Name = "Microchip Number")]
    public string? MicrochipNumber { get; set; }

    public string? ImageUrl { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Display(Name = "Sold Date")]
    [DataType(DataType.Date)]
    public DateTime? SoldDate { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    [NotMapped]
    public List<IFormFile>? ImageFiles { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Puppies")]
    public virtual Client? Client { get; set; }

    [ForeignKey("BreederId")]
    [InverseProperty("Puppies")]
    public virtual Breeder? Breeder { get; set; }

    [ForeignKey("LitterId")]
    [InverseProperty("Puppies")]
    public virtual Litter? Litter { get; set; }

    [InverseProperty("Puppy")]
    public virtual ICollection<Puppyrequest>? Puppyrequests { get; set; } = new List<Puppyrequest>();
}
