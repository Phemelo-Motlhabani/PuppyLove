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
        [Column(("BreedType"))]
        public int BreedId { get; set; }

        public ApplicationUser User { get; set; }
        public ActiveStatus Active {  get; set; }

        [ForeignKey("BreederId")]
        [InverseProperty("BreedSpecialization")]
        public virtual Breeder Breeder { get; set; }

        [ForeignKey("BreedId")]
        [InverseProperty("BreedSpecialization")]
        public virtual Breedtype Breedtype { get; set; }
    }
}
