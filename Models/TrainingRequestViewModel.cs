namespace PupV1.Models
{
    public class TrainingRequestViewModel
    {
        public string? TrequestId { get; set; }
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? TrainingProgram { get; set; }
        public string? RequestStatus { get; set; }

        public DateTime RequestDate { get; set; }
        public int DaysLeftToRespond {  get; set; }

    }
}
