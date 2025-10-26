using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> BrowseTrainers()
        {
            var trainers = await _context.Trainers
                .Include(t => t.Reviews)
                    .ThenInclude(r => r.Client)
                .Select(t => new ProviderWithReviewsViewModel
                {
                    ProviderId = t.TrainerId,
                    ProviderName = $"{t.Fname} {t.Lname}",
                    ProviderType = "Trainer",
                    ImageUrl = t.ImageUrl ?? "/images/default-profile.jpg",
                    City = t.City,
                    Email = t.Email,
                    Phone = t.CellNum,
                    AverageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0,
                    TotalReviews = t.Reviews.Count,
                    Reviews = t.Reviews.OrderByDescending(r => r.ReviewDate).Select(r => new ReviewDisplayViewModel
                    {
                        ReviewId = r.ReviewId,
                        ClientName = $"{r.Client.Fname} {r.Client.Lname}",
                        Rating = r.Rating,
                        ReviewText = r.ReviewText,
                        ReviewDate = r.ReviewDate
                    }).ToList()
                })
                .OrderByDescending(t => t.AverageRating)
                .ThenByDescending(t => t.TotalReviews)
                .ToListAsync();

            return View(trainers);
        }

        public async Task<IActionResult> BrowseBreeders()
        {
            var breeders = await _context.Breeders
                .Include(b => b.Reviews)
                    .ThenInclude(r => r.Client)
                .Select(b => new ProviderWithReviewsViewModel
                {
                    ProviderId = b.BreederId,
                    ProviderName = $"{b.Fname} {b.Lname}",
                    ProviderType = "Breeder",
                    ImageUrl = b.ImageUrl ?? "/images/default-profile.jpg",
                    City = b.City,
                    Email = b.Email,
                    Phone = b.CellNum,
                    KennelName = b.KennelName,
                    AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0,
                    TotalReviews = b.Reviews.Count,
                    Reviews = b.Reviews.OrderByDescending(r => r.ReviewDate).Select(r => new ReviewDisplayViewModel
                    {
                        ReviewId = r.ReviewId,
                        ClientName = $"{r.Client.Fname} {r.Client.Lname}",
                        Rating = r.Rating,
                        ReviewText = r.ReviewText,
                        ReviewDate = r.ReviewDate
                    }).ToList()
                })
                .OrderByDescending(b => b.AverageRating)
                .ThenByDescending(b => b.TotalReviews)
                .ToListAsync();

            return View(breeders);
        }

        [Authorize(Roles = "Client")]
        [HttpGet]
        public async Task<IActionResult> ReviewTrainer(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction("BrowseTrainers");
            }

            var user = await _userManager.GetUserAsync(User);

            
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == user.Email);

            if (client != null)
            {
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.TrainerId == id && r.ClientId == client.ClientId);

                if (existingReview != null)
                {
                    TempData["ErrorMessage"] = "You have already reviewed this trainer.";
                    return RedirectToAction("BrowseTrainers");
                }
            }

            var model = new ReviewViewModel
            {
                TrainerId = trainer.TrainerId,
                ProviderName = $"{trainer.Fname} {trainer.Lname}",
                ProviderType = "Trainer"
            };

            return View("CreateReview", model);
        }

        [Authorize(Roles = "Client")]
        [HttpGet]
        public async Task<IActionResult> ReviewBreeder(int id)
        {
            var breeder = await _context.Breeders.FindAsync(id);

            if (breeder == null)
            {
                TempData["ErrorMessage"] = "Breeder not found.";
                return RedirectToAction("BrowseBreeders");
            }

            var user = await _userManager.GetUserAsync(User);

            
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == user.Email);

            if (client != null)
            {
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.BreederId == id && r.ClientId == client.ClientId);

                if (existingReview != null)
                {
                    TempData["ErrorMessage"] = "You have already reviewed this breeder.";
                    return RedirectToAction("BrowseBreeders");
                }
            }

            var model = new ReviewViewModel
            {
                BreederId = breeder.BreederId,
                ProviderName = $"{breeder.Fname} {breeder.Lname}",
                ProviderType = "Breeder"
            };

            return View("CreateReview", model);
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(ReviewViewModel model)
        {
            Console.WriteLine("==================== POST CreateReview CALLED ====================");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            Console.WriteLine($"TrainerId: {model.TrainerId}");
            Console.WriteLine($"BreederId: {model.BreederId}");
            Console.WriteLine($"Rating: {model.Rating}");
            Console.WriteLine($"ReviewText Length: {model.ReviewText?.Length ?? 0}");
            Console.WriteLine($"ProviderType: {model.ProviderType}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model validation failed:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Any())
                    {
                        Console.WriteLine($"  {key}:");
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"    - {error.ErrorMessage}");
                            if (error.Exception != null)
                            {
                                Console.WriteLine($"    Exception: {error.Exception.Message}");
                            }
                        }
                    }
                }

                
                if (model.ProviderType == "Trainer" && model.TrainerId.HasValue)
                {
                    var trainer = await _context.Trainers.FindAsync(model.TrainerId.Value);
                    if (trainer != null)
                    {
                        model.ProviderName = $"{trainer.Fname} {trainer.Lname}";
                    }
                }
                else if (model.ProviderType == "Breeder" && model.BreederId.HasValue)
                {
                    var breeder = await _context.Breeders.FindAsync(model.BreederId.Value);
                    if (breeder != null)
                    {
                        model.ProviderName = $"{breeder.Fname} {breeder.Lname}";
                    }
                }

                return View(model);
            }

            
            var user = await _userManager.GetUserAsync(User);
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == user.Email);

            Console.WriteLine($"User: {user?.UserName}, Client: {client?.Fname} {client?.Lname}, ClientId: {client?.ClientId}");

            if (client == null)
            {
                Console.WriteLine("Client not found - redirecting to login");
                TempData["ErrorMessage"] = "Client profile not found. Please complete your profile.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var review = new Review
                {
                    ClientId = client.ClientId,
                    TrainerId = model.TrainerId,
                    BreederId = model.BreederId,
                    Rating = model.Rating,
                    ReviewText = model.ReviewText,
                    ReviewDate = DateTime.Now
                };

                Console.WriteLine("Adding review to context...");
                _context.Reviews.Add(review);

                Console.WriteLine("Saving changes...");
                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChanges returned: {result} (rows affected)");

                TempData["SuccessMessage"] = $"Thank you for reviewing {model.ProviderName}!";

                if (model.ProviderType == "Trainer")
                {
                    Console.WriteLine("Redirecting to BrowseTrainers");
                    return RedirectToAction("BrowseTrainers");
                }
                else
                {
                    Console.WriteLine("Redirecting to BrowseBreeders");
                    return RedirectToAction("BrowseBreeders");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", $"Error saving review: {ex.Message}");

                
                if (model.ProviderType == "Trainer" && model.TrainerId.HasValue)
                {
                    var trainer = await _context.Trainers.FindAsync(model.TrainerId.Value);
                    if (trainer != null)
                    {
                        model.ProviderName = $"{trainer.Fname} {trainer.Lname}";
                    }
                }
                else if (model.ProviderType == "Breeder" && model.BreederId.HasValue)
                {
                    var breeder = await _context.Breeders.FindAsync(model.BreederId.Value);
                    if (breeder != null)
                    {
                        model.ProviderName = $"{breeder.Fname} {breeder.Lname}";
                    }
                }

                return View(model);
            }
        }

        public async Task<IActionResult> GetTopRated()
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

            var viewModel = new TopRatedViewModel
            {
                TopTrainers = topTrainers,
                TopBreeders = topBreeders
            };

            return PartialView("_TopRatedSliders", viewModel);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}