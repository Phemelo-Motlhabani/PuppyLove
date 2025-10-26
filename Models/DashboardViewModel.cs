namespace PupV1.Models
{
    public class DashboardViewModel
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? CellNUm { get; set; }
        //public string? Email { get; set; }
        public string? TrainerID { get; set; }
        public string? Username { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile ImageFile { get; set; }
        // public string Password { get; set; }
        public List<TrainerSkillDisplayViewModel> SelectedSkills { get; set; } = new();
        public List<Trainingrequest> TrainingRequests { get; set; } = new List<Trainingrequest>();
        public List<TrainingProgress> TrainingProgresses { get; set; } = new List<TrainingProgress>();
    }
}
