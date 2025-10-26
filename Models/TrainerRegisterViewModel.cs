using System.ComponentModel.DataAnnotations;
namespace PupV1.Models
{
    public class TrainerRegisterViewModel
    {
        [Required]
        public int TrainerId { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? city { get; set; }
        public string? suburb {  get; set; }
        public string? CellNum { get; set; }
        public string username { get; set; }
        public IFormFile? ImageFile { get; set; }

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
