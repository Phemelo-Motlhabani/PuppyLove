using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Models;

[PrimaryKey("TrainerId", "SkillId")]
[Table("trainerskill")]
[Index("SkillId", Name = "fk_TrainerSkill_Skill")]
public partial class Trainerskill
{
    [Key]
    [Column("TrainerID")]
    public int TrainerId { get; set; }

    [Key]
    [Column("SkillID")]
    public int SkillId { get; set; }

    public ApplicationUser User { get; set; } 

    public string? SkillLevel { get; set; }

    [ForeignKey("SkillId")]
    [InverseProperty("Trainerskills")]
    public virtual Skill Skill { get; set; }

    [ForeignKey("TrainerId")]
    [InverseProperty("Trainerskills")]
    public virtual Trainer Trainer { get; set; }
}
