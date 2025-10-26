using System.ComponentModel.DataAnnotations;

namespace PupV1.Models
{
    public class ReviewViewModel
    {
        public int? TrainerId { get; set; }
        public int? BreederId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderType { get; set; }

        [Required(ErrorMessage = "Please select a rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public double Rating { get; set; }

        [Required(ErrorMessage = "Please write a comment")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 1000 characters")]
        [Display(Name = "Your Review")]
        public string ReviewText { get; set; }
    }
    public class ReviewDisplayViewModel
    {
        public int ReviewId { get; set; }
        public string ClientName { get; set; }
        public double Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
    }
    public class ProviderWithReviewsViewModel
    {
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderType { get; set; }
        public string? ImageUrl { get; set; }
        public string? City { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewDisplayViewModel> Reviews { get; set; } = new List<ReviewDisplayViewModel>();
        public string? Specialization { get; set; }
        public string? KennelName { get; set; }
    }
    public class TopRatedViewModel
    {
        public List<ProviderWithReviewsViewModel> TopTrainers { get; set; } = new List<ProviderWithReviewsViewModel>();
        public List<ProviderWithReviewsViewModel> TopBreeders { get; set; } = new List<ProviderWithReviewsViewModel>();
    }
}
