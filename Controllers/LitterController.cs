using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    [Authorize(Roles = "Breeder")]
    public class LitterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LitterController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
            {
                return RedirectToAction("Login", "Account"); 
            }
            var Litters = await _context.Litters
                .Where(l => l.BreederId == user.BreederId)
                .Include(l => l.BreedType)
                .Include(l => l.Puppies)
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync();

            return View(Litters);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var breeds =await _context.Breedtypes
                .OrderBy(b => b.BreedName)
                .ToListAsync();

            if(!breeds.Any())
            {
                TempData["ErrorMessage"] = "No breed types available.";
                return RedirectToAction("Index");
            }

            var model = new CreateLitterViewModel
            {
                AvailableBreeds = breeds,
                BirthDate = DateTime.Now,
                NumPuppies = 1
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLitterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
                model.AvailableBreeds = await _context.Breedtypes
                    .OrderBy(b => b.BreedName)
                    .ToListAsync();
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
                return RedirectToAction("Login", "Account");

            var litter = new Litter
            {
                BreederId = user.BreederId.Value,
                BreedId = model.BreedId,
                NumPuppies = model.NumPuppies,
                AvailablePuppies = model.NumPuppies,
                BirthDate = model.BirthDate,
                CreatedDate = DateTime.Now,
            };
            _context.Litters.Add(litter);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Litter created successfully!";
            return RedirectToAction("Details", new {id = litter.LitterId});
        }
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
                return RedirectToAction("Login", "Account");

            var litter = await _context.Litters
                .Include(l => l.BreedType)
                .Include(l => l.Puppies)
                .FirstOrDefaultAsync(l => l.LitterId == id && l.BreederId == user.BreederId);

            if (litter == null)
            {
                return NotFound();
            }

            var registeredPuppies = litter.Puppies.Count;
            var availablePuppies = litter.Puppies.Count(p =>
            p.Status == "Available" || string.IsNullOrEmpty(p.Status));
            var soldPuppies = litter.Puppies.Count(p => p.Status == "Sold");

            int ageInWeeks = litter.BirthDate.HasValue
                ? (int)((DateTime.Now - litter.BirthDate.Value).TotalDays/7)
                : 0;

            var viewModel = new LitterDetailsViewModel
            {
                LitterId = litter.LitterId,
                BreedName = litter.BreedType?.BreedName ?? "Unknown",
                TotalPuppies = litter.NumPuppies ?? 0,
                RegisteredPuppies = registeredPuppies,
                AvailablePuppies = availablePuppies,
                SoldPuppies = soldPuppies,
                BirthDate = litter.BirthDate,
                AgeInWeeks = ageInWeeks,
                Puppies = litter.Puppies.Select(p => new PuppyViewModel
                {
                    PuppyId = p.PuppyId,
                    PuppyName = p.PuppyName ?? "Unnamed",
                    Gender = p.Gender ?? "Unknown",
                    Weight = p.Weight,
                    Colour = p.Colour,
                    Size = p.Size,
                    Price = p.Price,
                    Status = p.Status ?? "Available",
                    IsVaccinated = p.Vaccinated == "Y",
                    IsMicrochipped = !string.IsNullOrEmpty(p.MicrochipNumber),
                    ImageUrl = p.ImageUrl ?? "/images/default-puppy.jpg",
                    DateOfBirth = p.DateOfBirth,
                    AgeInWeeks = p.DateOfBirth.HasValue
                        ? (int)((DateTime.Now - p.DateOfBirth.Value).TotalDays / 7)
                        : 0,
                    HealthStatus = p.HealthStatus
                }).OrderBy(p => p.PuppyName).ToList()
            };
            return View(viewModel); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var litter = await _context.Litters
                .Include(l => l.Puppies)
                .FirstOrDefaultAsync(l => l.LitterId == id && l.BreederId == user.BreederId);

            if(litter ==null)
            {
                return NotFound();
            }
            if(litter.Puppies.Any(p => p.Status == "Sold"))
            {
                TempData["ErrorMessage"] = "Cannot delete a litter with sold puppies";
                return RedirectToAction("Details", new { id });
            }
            _context.Litters.Remove(litter);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Litter deleted succesfully.";
            return RedirectToAction("Index");
        }
      
    }
}
