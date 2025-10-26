namespace PupV1.Models
{
    public class HomePageViewModel
    {
        public TopRatedViewModel topRated { get; set; } = new TopRatedViewModel();
        public List<Parkrecommendation> ParkRecommendations { get; set; } = new List<Parkrecommendation>();
    }
}
