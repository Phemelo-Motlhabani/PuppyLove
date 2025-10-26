namespace PupV1.Models
{
    public class ClientDashboardViewModel
    {
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? City { get; set; }
        public string? Suburb { get; set; }
        public string? CellNum { get; set; }
        public string? Username { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }

        public List<Trainingrequest> TrainingRequests { get; set; }
    }
}
