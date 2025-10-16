namespace PupV1.Models
{
    public class SkillSelectionViewModel
    {
        public int SkillId { get; set; }
        public string? SkillName { get; set; }
        public bool IsSelected { get; set; }
        public string? SkillLevel { get; set; } = "Beginner";

    }
}
