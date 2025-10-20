using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PupV1.Models;
using PupV1.Data;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;

namespace PupV1.Controllers
{
    [Authorize(Roles = "Breeder")]
    public class PuppyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PuppyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Register(int? litterId)
        {
            var user = await _userManager.GetUserAsync(User);

            if ((user?.BreederId == null))
            {
                return RedirectToAction("Login", "Account");
            }

            var litters = await _context.Litters
                .Where(l => l.BreederId == user.BreederId)
                .Include(l => l.BreedType)
                .Include(l => l.Puppies)
                .ToListAsync();

            var availableLitters = litters
                .Where(l => l.Puppies.Count < (l.NumPuppies ?? 0))
                .Select(l => new LitterDropdownItem
                {
                    LitterId = l.LitterId,
                    DisplayText = $"{l.BreedType?.BreedName} - Born {l.BirthDate?.ToString("MM/dd/yyyy")} ({l.Puppies.Count}/{l.NumPuppies} registered)"
                }).ToList();

            if(!availableLitters.Any())
            {
                TempData["ErrorMessage"] = "All litters a full, create new";
                return RedirectToAction("Create", "Litter");
            }

            var selectedLitter = litters.FirstOrDefault(l => l.LitterId == litterId) ?? litters.First();

            var model = new RegisterPuppyViewModel
            {
                AvailableLitters = availableLitters,
                LitterId = litterId ?? availableLitters.First().LitterId,
                DateOfBirth = selectedLitter.BirthDate ?? DateTime.Now.Date,
                IsVaccinated = false
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterPuppyViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                await PopulateLitterDropdown(model, user.BreederId.Value);
                return View(model);
            }
            var litter = await _context.Litters
                .Include(l => l.Puppies)
                .FirstOrDefaultAsync(l => l.LitterId == model.LitterId && l.BreederId == user.BreederId);

            if (litter == null)
            {
                ModelState.AddModelError("", "Invalid litter selected.");
                await PopulateLitterDropdown(model, user.BreederId.Value);
                return View(model);
            }
            if (litter.Puppies.Count >= (litter.NumPuppies ?? 0))
            {
                ModelState.AddModelError("", "This litter already has the maximum number of puppies registered.");
                await PopulateLitterDropdown(model, user.BreederId.Value);
                return View(model);
            }

            string? imageUrl = null;
            if (model.ImageFiles != null && model.ImageFiles.Any())
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "puppies");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var firstImage = model.ImageFiles.First();
                if (firstImage.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + firstImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await firstImage.CopyToAsync(fileStream);
                    }

                    imageUrl = "/images/puppies/" + uniqueFileName;
                }

            }
            int ageInWeeks = (int)((DateTime.Now - model.DateOfBirth).TotalDays / 7);

            var puppy = new Puppy
            {
                LitterId = model.LitterId,
                PuppyName = model.PuppyName,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Age = ageInWeeks,
                Weight = model.Weight,
                Colour = model.Colour,
                Size = model.Size,
                Price = model.Price,
                HealthStatus = model.HealthStatus,
                Vaccinated = model.IsVaccinated ? "Y" : "N",
                VaccinationDate = model.VaccinationDate,
                MicrochipNumber = model.IsMicrochipped ?model.MicrochipNumber : null,
                Status = "Available",
                ImageUrl = imageUrl ?? "/images/default-puppy.jpg",
                CreatedDate = DateTime.Now
            };
            _context.Puppies.Add(puppy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{model.PuppyName} has been registered.";
            return RedirectToAction("Details", "Litter", new { id = model.LitterId });

        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
                return RedirectToAction("Login", "Account");

            var puppy = await _context.Puppies
                .Include(p => p.Litter)
                    .ThenInclude(l => l.BreedType)
                .Include(p => p.Litter)
                    .ThenInclude(l => l.Breeder)
                .FirstOrDefaultAsync(p => p.PuppyId == id);

            if (puppy == null || puppy.Litter?.BreederId != user.BreederId)
            {
                return NotFound();
            }
                

            return View(puppy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsSold(int id)
        {
            var user = await _userManager.GetUserAsync (User);
            if (user?.BreederId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var puppy = await _context.Puppies
                .Include(p => p.Litter)
                .FirstOrDefaultAsync(p => p.PuppyId == id);

            if(puppy == null || puppy.Litter?.BreederId != user.BreederId)
            {
                return NotFound();
            }
            if (puppy.Status == "Sold")
            {
                TempData["ErrorMessage"] = "This puppy is already marked as sold.";
                return RedirectToAction("Details", new { id });
            }

            puppy.Status = "Sold";
            puppy.SaleStatus = "S";
            puppy.SoldDate = DateTime.Now;

            if (puppy.Litter != null && puppy.Litter.AvailablePuppies > 0)
            {
                puppy.Litter.AvailablePuppies--;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{puppy.PuppyName} has been marked as sold!";
            return RedirectToAction("Details", "Litter", new { id = puppy.LitterId });
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
               
            var puppy = await _context.Puppies
                .Include(p => p.Litter)
                .FirstOrDefaultAsync(p => p.PuppyId == id);

            if (puppy == null || puppy.Litter?.BreederId != user.BreederId)
            {
                return NotFound();
            }
            if (puppy.Status == "Sold")
            {
                TempData["ErrorMessage"] = "Cannot delete a sold puppy.";
                return RedirectToAction("Details", new { id });
            }

            var litterId = puppy.LitterId;
            if (!string.IsNullOrEmpty(puppy.ImageUrl) && puppy.ImageUrl != "/images/default-puppy.jpg")
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, puppy.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _context.Puppies.Remove(puppy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{puppy.PuppyName} has been removed.";
            return RedirectToAction("Details", "Litter", new { id = litterId });
        }
        private async Task PopulateLitterDropdown(RegisterPuppyViewModel model, int breederId)
        {
            var litters = await _context.Litters
                .Where(l => l.BreederId == breederId)
                .Include(l => l.BreedType)
                .Include(l => l.Puppies)
                .ToListAsync();

            model.AvailableLitters = litters
                .Where(l => l.Puppies.Count < (l.NumPuppies ?? 0))
                .Select(l => new LitterDropdownItem
                {
                    LitterId = l.LitterId,
                    DisplayText = $"{l.BreedType?.BreedName} - Born {l.BirthDate?.ToString("MM/dd/yyyy")} ({l.Puppies.Count}/{l.NumPuppies} registered)"

                }).ToList();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
