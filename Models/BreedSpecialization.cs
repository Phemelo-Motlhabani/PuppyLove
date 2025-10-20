using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace PupV1.Models
{
    [PrimaryKey("BreederId", "BreedId")]
    [Table("breedspecialiation")]
    [Index("BreedId", Name = "fk_BreedSpecialization_Specialization")]
    public class BreedSpecialization
    {
        [Key]
        [Column("Breeder")]
        public int BreederId { get; set; }

        [Key]
        [Column(("Breedtype"))]
        public int BreedId { get; set; }

        public ApplicationUser User { get; set; }
        public bool Active {  get; set; }

        [ForeignKey("BreederId")]
        [InverseProperty("BreedSpecializations")]
        public virtual Breeder Breeder { get; set; }

        [ForeignKey("BreedId")]
        [InverseProperty("BreedSpecializations")]
        public virtual Breedtype Breedtype { get; set; }
    }
}
