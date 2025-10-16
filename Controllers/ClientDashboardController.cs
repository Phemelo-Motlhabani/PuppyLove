using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    [Authorize(Roles ="Client")]
    public class ClientDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        /*public IActionResult Index()
        {
            return View();
        }*/
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // User = ClaimsPrincipal from HttpContext
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Not logged in
            }
            Client client = null;

            if(user.ClientId.HasValue)
            {
                client = await _context.Clients.FirstOrDefaultAsync(c =>  c.ClientId == user.ClientId.Value);
            }
            if (client == null)
            {
                client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == user.Email);

                if (client != null && !user.ClientId.HasValue)
                {
                    user.ClientId = client.ClientId;
                    await _userManager.UpdateAsync(user);
                }
            }
            if (client == null)
            {
                TempData["Error"] = "Client profile not found.";
                return RedirectToAction("Index", "Home");
            }
            var trainingRequests = await _context.Trainingrequests
                .Where(tr => tr.ClientId == user.ClientId)
                .ToListAsync();
            var viewModel = new ClientDashboardViewModel
            {
                Fname = client.Fname,
                Lname = client.Lname,
                City = client.City,
                Suburb = client.Suburb,
                CellNum = client.CellNum,
                Username = client.Username,
                TrainingRequests = trainingRequests
            };

            return View(viewModel);
            /*var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == user.Email);

            return View(client);*/
        }

        public async Task<IActionResult> Puppies()
        {
            return View(await _context.Puppies.ToListAsync());
        }

        public async Task<IActionResult> Puppy(int id)
        {
            var puppy = await _context.Puppies.FirstOrDefaultAsync(p => p.PuppyId == id);

            if (puppy == null)
            {
                TempData["Error"] = "Puppy not found.";
                return RedirectToAction("Puppies");
            }

            var litter = await _context.Litters.FirstOrDefaultAsync(l => l.LitterId == puppy.LitterId);
            if (litter == null)
            {
                TempData["Error"] = "Litter information not found for this puppy.";
                return RedirectToAction("Puppies");
            }

            var breeder = await _context.Breeders.FirstOrDefaultAsync(b => b.BreederId == litter.BreederId);
            if (breeder == null)
            {
                TempData["Error"] = "Breeder information not found.";
                return RedirectToAction("Puppies");
            }

            var breedType = await _context.Breedtypes.FirstOrDefaultAsync(bt => bt.BreedId == litter.BreedId);
            if (breedType == null)
            {
                TempData["Error"] = "Breed type information not found.";
                return RedirectToAction("Puppies");
            }
            /*if (id == null)
            {
                return NotFound();
            }

            var puppy = await _context.Puppies.FirstOrDefaultAsync(p => p.PuppyId == id);
            var litter = await _context.Litters.FirstOrDefaultAsync(l => l.LitterId == puppy.LitterId);
            var breeder = await _context.Breeders.FirstOrDefaultAsync(b => b.BreederId == litter.BreederId);
            var breedType = await _context.Breedtypes.FirstOrDefaultAsync(bt => bt.BreedId == litter.BreedId);*/

            var puppyDetails = new PuppyDetails();
            puppyDetails.PuppyId = puppy.PuppyId;
            puppyDetails.Name = puppy.PuppyName;
            puppyDetails.Size = breedType.Size;
            puppyDetails.Colour = puppy.Colour;
            puppyDetails.BreederName = breeder.KennelName;
            puppyDetails.BreederId = breeder.BreederId;
            puppyDetails.Gender = puppy.Gender;
            puppyDetails.Breed = breedType.BreedName;
            puppyDetails.ImageUrl = puppy.ImageUrl;
            puppyDetails.Price = puppy.Price;

            if (puppy == null)
            {
                return NotFound();
            }

            return View(puppyDetails);
        }

        [HttpPost]
        public async Task<IActionResult> RequestPuppy(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User); // User = ClaimsPrincipal from HttpContext
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Not logged in
            }

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == user.Email);
            var puppy = await _context.Puppies.FirstOrDefaultAsync(p => p.PuppyId == id);
            var litter = await _context.Litters.FirstOrDefaultAsync(l => l.LitterId == puppy.LitterId);
            var breeder = await _context.Breeders.FirstOrDefaultAsync(b => b.BreederId == litter.BreederId);
            var breedType = await _context.Breedtypes.FirstOrDefaultAsync(bt => bt.BreedId == litter.BreedId);

            var puppyRequest = new Puppyrequest();
            puppyRequest.RequestId = GeneratePRequestId();
            puppyRequest.Status = "P";
            puppyRequest.ExpDate = DateTime.Today.AddDays(14);
            puppyRequest.ClientId = client.ClientId;
            puppyRequest.BreederId = breeder.BreederId;

            _context.Add(puppyRequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your request has been submitted successfully!";
            return RedirectToAction(nameof(Puppy), new { id = puppy.PuppyId }); ;
        }

        public int GeneratePRequestId()
        {
            int i = 1;
            while (true)
            {
                if (!PRequestExists(i))
                {
                    return i;
                }
                i++;
            }
        }

        private bool PRequestExists(int id)
        {
            return _context.Puppyrequests.Any(e => e.RequestId == id);
        }

        public async Task<IActionResult> Breeder(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var breeder = await _context.Breeders.FirstOrDefaultAsync(b => b.BreederId == id);

            var breeder = await _context.Breeders
                .Include(b => b.Reviews)              // load reviews
                .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(b => b.BreederId == id);

            if (breeder == null)
            {
                return NotFound();
            }

            return View(breeder);
        }

        public async Task<IActionResult> Trainer(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var breeder = await _context.Breeders.FirstOrDefaultAsync(b => b.BreederId == id);

            var trainer = await _context.Trainers
                .Include(t => t.Reviews)              // load reviews
                .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(t => t.TrainerId == id);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        public async Task<IActionResult> Trainers()
        {
            var trainers = _context.Trainers
                .Include(t => t.Trainerskills)
                .ThenInclude(r => r.Skill);
            return View(await trainers.ToListAsync());
        }

        public async Task<IActionResult> TrainerReview(int? id)
        {
            var trainer = await _context.Trainers
                .Include(t => t.Reviews)              // load reviews
                .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(t => t.TrainerId == id);

            return View(trainer);
        }

        public async Task<IActionResult> BreederReview(int? id)
        {
            var breeder = await _context.Breeders
                .Include(b => b.Reviews)              // load reviews
                .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(b => b.BreederId == id);

            return View(breeder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostReview(int breederId, int trainerId, int rating, string reviewText)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == user.Email);

            if (client == null)
            {
                TempData["Error"] = "You must be a registered client to post reviews.";
                if (breederId != null)
                    return RedirectToAction("Breeder", new { id = breederId });
                if (trainerId != null)
                    return RedirectToAction("Trainer", new { id = trainerId });
            }

            var review = new Review
            {
                ReviewId = GenerateReviewId(),
                ClientId = client.ClientId,
                BreederId = breederId,
                TrainerId = trainerId,
                Rating = rating,
                ReviewText = reviewText,
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            if (breederId != null)
            {
                TempData["Success"] = "Your review has been submitted!";
                return RedirectToAction("Breeder", new { id = breederId });
            }

            if (trainerId != null)
            {
                TempData["Success"] = "Your review has been submitted!";
                return RedirectToAction("Trainer", new { id = trainerId });
            }

            return NotFound();
        }


        public int GenerateReviewId()
        {
            int i = 1;
            while (true)
            {
                if (!ReviewExists(i))
                {
                    return i;
                }
                i++;
            }
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }

        public async Task<IActionResult> RequestTraining(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User); // User = ClaimsPrincipal from HttpContext
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Not logged in
            }

            var trainer = await _context.Trainers
                .Include(t => t.Reviews)              // load reviews
                .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(t => t.TrainerId == id);

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == user.Email);
            var puppy = await _context.Puppies.FirstOrDefaultAsync(p => p.PuppyId == id);
            var litter = await _context.Litters.FirstOrDefaultAsync(l => l.LitterId == puppy.LitterId);
            var breeder = await _context.Breeders.FirstOrDefaultAsync(b => b.BreederId == litter.BreederId);
            var breedType = await _context.Breedtypes.FirstOrDefaultAsync(bt => bt.BreedId == litter.BreedId);

            var trainingRequest = new Trainingrequest();
            trainingRequest.TrequestId = GenerateTRequestId();
            trainingRequest.RequestDate = DateTime.Today.AddDays(14);
            trainingRequest.ClientId = client.ClientId;

            _context.Add(trainingRequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your request has been submitted successfully!";
            return RedirectToAction(nameof(Puppy), new { id = puppy.PuppyId }); ;
        }

        public int GenerateTRequestId()
        {
            int i = 1;
            while (true)
            {
                if (!TRequestExists(i))
                {
                    return i;
                }
                i++;
            }
        }

        private bool TRequestExists(int id)
        {
            return _context.Trainingrequests.Any(e => e.TrequestId == id);
        }
    }
}
