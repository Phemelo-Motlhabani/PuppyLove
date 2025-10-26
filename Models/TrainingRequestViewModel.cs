using System.ComponentModel.DataAnnotations;

namespace PupV1.Models
{
    public class CreateTrainingRrequestViewModel
    {
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
        public string? TrainerCity { get; set; }

        [Required(ErrorMessage = "Please select a training program")]
        [Display(Name = "Training Program")]
        public string TrainingProgram { get; set; }
    }
    public class TrainingRequestViewModel
    {

        public string? TrequestId { get; set; }
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? DogName { get; set; }
        public string? DogBreed  { get; set; }
        public string? ClientPhone { get; set; }
        public int TrainerId { get; set; }
        public string? TrainerName { get;set; }
        public string? TrainingProgram { get; set; }
        public string? RequestStatus { get; set; }
        public string? AdditionalInfo { get; set; }

        public DateTime RequestDate { get; set; }
        public int DaysLeftToRespond {  get; set; }

    }
    public class RespondToTrainingRequestViewModel
    {
        public int TrequestId { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
