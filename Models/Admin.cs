using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("admin")]
public partial class Admin
{
    [Key]
    [Column("AdminID")]
    public int AdminId { get; set; }

    [StringLength(20)]
    public string? Username { get; set; }

    [Column("FName")]
    [StringLength(20)]
    public string? Fname { get; set; }

    [Column("LName")]
    [StringLength(20)]
    public string? Lname { get; set; }

    [StringLength(20)]
    public string? Email { get; set; }

    public int? CellNum { get; set; }

    [StringLength(25)]
    public string? City { get; set; }

    [StringLength(25)]
    public string? Suburb { get; set; }

    public int? PostCode { get; set; }

    [StringLength(25)]
    public string? Password { get; set; }
}
