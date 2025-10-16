using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PupV1.Models
{
    public class TrainingProgress
    {
        [Key]
        public int ProgressId { get; set; }
        
        [StringLength(100)]
        public string DogName { get; set; }

        
        [StringLength(100)]
        public string DogBreed { get; set; }

        
        [StringLength(100)]
        public string? OwnerName { get; set; }
        public int TrainerId { get; set; }

        
        public string? Program { get; set; }
        public string ProgressNotes { get; set; }
        public bool IsFinished { get; set; } = false;

        [ForeignKey("TrainerId")]
        //[InverseProperty("TrainingProgesses")]
        public virtual Trainer? Trainer { get; set; }
    }
}
