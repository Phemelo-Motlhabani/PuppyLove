using System.ComponentModel.DataAnnotations;

namespace PupV1.Models
{
    public class BreederRegisterViewModel
    {
        [Required]
        public int BreederId { get; set; }
        public string? Username { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? CellNum { get; set; }
        public string? City { get; set; }
        public string? Suburb { get; set; }
        public string? KennelName { get; set; }
        public string? LicenceNum { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirmation { get; set; } = string.Empty;
    }
}
