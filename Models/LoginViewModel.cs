using System.ComponentModel.DataAnnotations;

namespace PupV1.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name ="Username")]
        public string username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "Remember Me")]
        public bool rememberMe { get; set; }
    }
}
