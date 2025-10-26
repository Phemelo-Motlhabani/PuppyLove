using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PupV1.Models
{
    public class RegisterPuppyViewModel
    {
        [Required]
        public int LitterId { get; set; }
        [Required]
        public string PuppyName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }

        [Range(0.1, 100)]
        [Display(Name ="Weight (kg)")]
        public decimal? Weight { get; set; }

        [Required]
        public string Colour { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        [Range(0, 1000000)]
        [Display(Name = "Price (R)")]
        public decimal Price { get; set; }
        public string? HealthStatus { get; set; }
        public bool IsVaccinated { get; set; }
        public bool IsMicrochipped { get; set; }
        public DateTime? VaccinationDate { get; set; }
        public string? MicrochipNumber { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Puppy Images")]
        public List<IFormFile>? ImageFiles { get; set; }

        public List<LitterDropdownItem>? AvailableLitters { get; set; }

    }
    public class LitterDropdownItem
    {
        public int LitterId { get; set; }
        public string DisplayText { get; set; }

    }
    public class PuppyViewModel
    {
        public int PuppyId { get; set; }
        public string PuppyName { get; set; }
        public string Gender { get; set; }
        public decimal? Weight { get; set; }
        public string? Colour { get; set; }
        public string? Size { get; set; }
        public decimal? Price { get; set; }
        public string Status { get; set; }
        public bool IsVaccinated { get; set; }
        public bool IsMicrochipped { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int AgeInWeeks { get; set; }
        public string? HealthStatus { get; set; }
    }

}
