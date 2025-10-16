using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[Table("skill")]
public partial class Skill
{
    [Key]
    [Column("SkillID")]
    public int SkillId { get; set; }

    [StringLength(100)]
    public string? SkillName { get; set; }

    [InverseProperty("Skill")]
    public virtual ICollection<Trainerskill> Trainerskills { get; set; } = new List<Trainerskill>();
}
