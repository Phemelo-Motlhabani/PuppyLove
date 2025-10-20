using System.Drawing;

namespace PupV1.Models
{
    public class BreederDashboardViewModel
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? CellNUm { get; set; }
        public int BreederId { get; set; }
        public string? KennelName { get; set; }
        public string? LicenceNum { get; set; }
        public string? Username { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
