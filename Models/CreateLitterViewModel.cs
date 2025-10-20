using System.ComponentModel.DataAnnotations;


namespace PupV1.Models
{
    public class CreateLitterViewModel
    {
        [Required]
        [Display(Name = "Breed Type")]
        public int BreedId { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Number of puppies must be between 1 and 20")]
        [Display(Name = "Number of Puppies")]
        public int NumPuppies { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Additional Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        public List<Breedtype>? AvailableBreeds { get; set; }
    }
    public class LitterDetailsViewModel
    {
        public int LitterId { get; set; }
        public string BreedName { get; set; }
        public int TotalPuppies { get; set; }
        public int AvailablePuppies { get; set; }
        public int SoldPuppies { get; set; }
        public int RegisteredPuppies { get; set; }
        public DateTime? BirthDate { get; set; }
        public int AgeInWeeks { get; set; }
        public string? Notes { get; set; }
        public List<PuppyViewModel> Puppies { get; set; } = new List<PuppyViewModel>();
    }
}
