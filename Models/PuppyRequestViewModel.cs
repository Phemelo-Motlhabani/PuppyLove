using System;
using System.ComponentModel.DataAnnotations;
using static System.Console;



namespace PupV1.Models
{
    public class PuppyRequestViewModel
    {
        public int PuppyId { get; set; }
        public string? PuppyName { get; set; }
        public string? BreedName { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Please provide a message")]
        [StringLength(1000)]
        [Display(Name = "Message to Breeder")]
        public string? Message { get; set; }
    }

    public class PuppyRequestDetailsViewModel
    {
        public int RequestId { get; set; }
        public int PuppyId { get; set; }
        public string PuppyName { get; set; }
        public string BreedName { get; set; }
        public decimal? Price { get; set; }
        public string? PuppyImageUrl { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public string BreederName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string? BreederResponse { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ResponseDate { get; set; }
    }

    public class ManageRequestViewModel
    {
        public int RequestId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Accepted or Rejected

        [StringLength(1000)]
        [Display(Name = "Response Message")]
        public string? BreederResponse { get; set; }
    }
}
