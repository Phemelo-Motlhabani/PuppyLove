using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;
using System.Diagnostics;

namespace PupV1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        
        {
            _context = context;
            _logger = logger;
        }

       

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult>Index()
        {
            var topTrainers = await _context.Trainers
                .Include(t => t.Reviews)
                .Where(t => t.Reviews.Any())
                .Select(t => new ProviderWithReviewsViewModel
                {
                    ProviderId = t.TrainerId,
                    ProviderName = $"{t.Fname} {t.Lname}",
                    ProviderType = "Trainer",
                    ImageUrl = t.ImageUrl ?? "/images/default-profile.jpg",
                    City = t.City,
                    AverageRating = t.Reviews.Average(r => r.Rating),
                    TotalReviews = t.Reviews.Count
                })
                .OrderByDescending(t => t.AverageRating)
                .ThenByDescending(t => t.TotalReviews)
                .Take(5)
                .ToListAsync();

            var topBreeders = await _context.Breeders
                .Include(b => b.Reviews)
                .Where(b => b.Reviews.Any())
                .Select(b => new ProviderWithReviewsViewModel
                {
                    ProviderId = b.BreederId,
                    ProviderName = $"{b.Fname} {b.Lname}",
                    ProviderType = "Breeder",
                    ImageUrl = b.ImageUrl ?? "/images/default-profile.jpg",
                    City = b.City,
                    KennelName = b.KennelName,
                    AverageRating = b.Reviews.Average(r => r.Rating),
                    TotalReviews = b.Reviews.Count
                })
                .OrderByDescending(b => b.AverageRating)
                .ThenByDescending(b => b.TotalReviews)
                .Take(5)
                .ToListAsync();

            var parks = await _context.Parkrecommendations.ToListAsync();
            var viewModel = new HomePageViewModel
                {
                topRated = new TopRatedViewModel
                {
                    TopTrainers = topTrainers,
                    TopBreeders = topBreeders
                },
                ParkRecommendations = parks
            };
            return View(viewModel);
            
        }
    }
}
