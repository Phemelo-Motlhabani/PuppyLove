using Microsoft.AspNetCore.Mvc;
using PupV1.Data;
using PupV1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace PupV1.Controllers
{
    public class BreederDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BreederDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Dashboard(string sizeFilter, int? breedFilter)
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // A500: Generate BreedType Rankings based on sales count
            var rankings = await _context.breedSpecializations
                .Where(bs => bs.BreederId == currentBreeder.BreederId && bs.Active == ActiveStatus.YES)
                .Include(bs => bs.BreedType)
                .Select(bs => bs.BreedType)
                .OrderByDescending(bt => bt.SaleCount)
                .ToListAsync();

            // Query for the breeder's available puppies
            var puppiesQuery = _context.Puppies
                .Include(p => p.Litter)
                .ThenInclude(l => l.BreedType)
                .Where(p => p.Litter.BreederId == currentBreeder.BreederId && p.SaleStatus == SaleStatus.Available);

            // Filters
            if (!string.IsNullOrEmpty(sizeFilter) && Enum.TryParse<Size>(sizeFilter, out var sizeEnum))
            {
                puppiesQuery = puppiesQuery.Where(p => p.Litter.BreedType.Size == sizeEnum);
            }
            if (breedFilter.HasValue)
            {
                puppiesQuery = puppiesQuery.Where(p => p.Litter.BreedID == breedFilter.Value);
            }

            var filteredPuppies = await puppiesQuery
                .OrderByDescending(p => p.Litter.BreedType.SaleCount)
                .ThenBy(p => p.PuppyName)
                .ToListAsync();

            // Get litters and requests for the breeder
            var litters = await _context.Litters
                .Where(l => l.BreederId == currentBreeder.BreederId)
                .Include(l => l.BreedType)
                .Include(l => l.Puppies)
                .ToListAsync();

            var puppyRequests = await _context.PuppyRequests
                .Where(pr => pr.BreederID == currentBreeder.BreederId && pr.Status == RequestStatus.Pending)
                .Include(pr => pr.Client)
                .Include(pr => pr.Puppy)
                .ToListAsync();

            var breedspecializations = await _context.breedSpecializations
                .Where(bs => bs.BreederId == currentBreeder.BreederId && bs.Active == ActiveStatus.YES)
                .Include(bs => bs.BreedType)
                .ToListAsync();

            var viewModel = new BreederDashboardViewModel
            {
                Breeder = currentBreeder,
                BreedTypeRankings = rankings,
                AvailablePuppies = filteredPuppies,
                Litters = litters,
                PuppyRequests = puppyRequests,
                BreedSpecializations = breedSpecializations,
                SizeOptions = new SelectList(Enum.GetNames(typeof(Size))),
                BreedOptions = new SelectList(await _context.Breedtypes.ToListAsync(), "BreedID", "BreedName")
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ManageBreedTypes()
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var allBreeds = await _context.Breedtypes.ToListAsync();
            var breederSpecializations = await _context.breedSpecializations
                .Where(bs => bs.BreederId == currentBreeder.BreederId)
                .Include(bs => bs.Breedtype)
                .ToListAsync();

            var viewModel = new ManageBreedTypesViewModel
            {
                AllBreeds = allBreeds,
                BreederSpecializations = breederSpecializations
            };

            return View(viewModel);
        }

        // A200: Update Breeder Details - GET
        [HttpGet]
        public async Task<IActionResult> UpdateBreederDetails()
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var viewModel = new UpdateBreederDetailsViewModel
            {
                FName = currentBreeder.FName,
                LName = currentBreeder.LName,
                Email = currentBreeder.Email,
                CellNum = currentBreeder.CellNum,
                City = currentBreeder.City,
                Suburb = currentBreeder.Suburb,
                KennelName = currentBreeder.KennelName
            };

            return View(viewModel);
        }

        // A200: Update Breeder Details - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBreederDetails(UpdateBreederDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentBreeder = await GetCurrentBreederAsync();
                if (currentBreeder == null) return RedirectToAction("Login", "Account");

                currentBreeder.FName = model.FName;
                currentBreeder.LName = model.LName;
                currentBreeder.Email = model.Email;
                currentBreeder.CellNum = model.CellNum;
                currentBreeder.City = model.City;
                currentBreeder.Suburb = model.Suburb;
                currentBreeder.KennelName = model.KennelName;

                _context.Update(currentBreeder);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your details have been updated successfully!";
                return RedirectToAction(nameof(Dashboard));
            }
            return View(model);
        }

        // A300: Add Litter - GET 
        [HttpGet]
        public async Task<IActionResult> AddLitter(AddLitterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentBreeder = await GetCurrentBreederAsync();
                if (currentBreeder == null) return RedirectToAction("Login", "Account");

                // Verify the breeder specializes in this breed
                var hasSpecialization = await _context.breedSpecializations
                    .AnyAsync(bs => bs.BreederId == currentBreeder.BreederId &&
                                   bs.BreedId == model.BreedId &&
                                   bs.Active == ActiveStatus.YES);
                if (!hasSpecialization)
                {
                    ModelState.AddModelError("", "You can only create litters for breeds you specialize in.");
                    return View(model);
                }
                // Only show breeds the breeder specializes in and are active
                var availableBreeds = await _context.breedSpecializations
                .Where(bs => bs.BreederId == currentBreeder.BreederId && bs.Active == ActiveStatus.YES)
                .Include(bs => bs.BreedType)
                .Select(bs => bs.BreedType)
                .ToListAsync();

                var viewModel = new AddLitterViewModel
                {
                    AvailableBreedTypes = availableBreeds.Select(bt => new SelectListItem
                    {
                        Value = bt.BreedID.ToString(),
                        Text = bt.BreedName.ToString()
                    }).ToList()
                };

                return View(viewModel);
            }
            return RedirectToAction(nameof(Dashboard));
        }

        // A300: Add Litter - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("AddLitter")]
        public async Task<IActionResult> AddLitterPost(AddLitterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentBreeder = await GetCurrentBreederAsync();
                if (currentBreeder == null) return RedirectToAction("Login", "Account");

                var litter = new Litter
                {
                    BreedId = model.BreedID,
                    BreederId = currentBreeder.BreederId,
                    NumPuppies = model.NumPuppies
                };

                _context.Add(litter);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Litter added successfully. Now register the individual puppies.";
                return RedirectToAction("RegisterPuppies", new { litterId = litter.LitterId });
            }

            // Reload dropdown if the model is invalid
            var currentBreeder2 = await GetCurrentBreederAsync();
            var availableBreeds = await _context.breedSpecializations
                .Where(bs => bs.BreederId == currentBreeder2.BreederId && bs.Active == ActiveStatus.YES)
                .Include(bs => bs.BreedType)
                .Select(bs => bs.BreedType)
                .ToListAsync();

            model.AvailableBreedTypes = availableBreeds.Select(bt => new SelectListItem
            {
                Value = bt.BreedID.ToString(),
                Text = bt.BreedName.ToString()
            }).ToList();

            return View(model);
        }

        // A310: Remove Litter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLitter(int litterId)
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var litter = await _context.Litters
                .Include(l => l.Puppies)
                .FirstOrDefaultAsync(l => l.LitterId == litterId && l.BreederId == currentBreeder.BreederId);

            if (litter == null)
            {
                TempData["ErrorMessage"] = "Litter not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Can only delete litter with zero puppies
            if (litter?.Puppies != null && (!litter.Puppies.Any() || litter.NumPuppies <= 0))
            {
                TempData["ErrorMessage"] = "Cannot remove a litter that still has puppies registered to it.";
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                _context.Litters.Remove(litter);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Litter successfully removed.";
            }
            return RedirectToAction(nameof(Dashboard));
        }

        // A400: View Puppy Requests
        public async Task<IActionResult> ViewPuppyRequests()
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var requests = await _context.PuppyRequests
                .Include(pr => pr.Client)
                .Include(pr => pr.Puppy)
                .ThenInclude(p => p.Litter.BreedType)
                .Where(pr => pr.BreederID == currentBreeder.BreederId && pr.Status == RequestStatus.Pending)
                .ToListAsync();

            return View(requests);
        }

        // A410: Accept Puppy Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptPuppyRequest(int requestId)
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var request = await _context.PuppyRequests
                .Include(pr => pr.Puppy)
                .ThenInclude(p => p.Litter)
                .ThenInclude(l => l.BreedType)
                .FirstOrDefaultAsync(pr => pr.RequestID == requestId && pr.BreederID == currentBreeder.BreederID);

            if (request?.Puppy?.Litter != null)
            {
                var puppy = request.Puppy;
                var litter = puppy.Litter;

                // Update puppy ownership and status
                puppy.ClientID = request.ClientID;
                puppy.SaleStatus = SaleStatus.Sold;
                request.Status = RequestStatus.Accepted;

                // Decrement litter puppy count
                litter.NumPuppies = Math.Max(0, litter.NumPuppies - 1);

                // Update breed sale count for rankings
                var breedType = litter.BreedType;
                breedType.SaleCount++;

                // Update all entities
                _context.Update(puppy);
                _context.Update(request);
                _context.Update(litter);
                _context.Update(breedType);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Puppy {puppy.PuppyName} has been sold successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Request not found or invalid.";
            }

            return RedirectToAction(nameof(ViewPuppyRequests));
        }

        // Reject Puppy Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPuppyRequest(int requestId)
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var request = await _context.PuppyRequests
                .Include(pr => pr.Client)
                .Include(pr => pr.Puppy)
                .FirstOrDefaultAsync(pr => pr.RequestID == requestId && pr.BreederID == currentBreeder.BreederID);

            if (request != null)
            {
                request.Status = RequestStatus.Rejected;
                _context.Update(request);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Request from {request.Client.FName} {request.Client.LName} has been rejected.";
            }
            else
            {
                TempData["ErrorMessage"] = "Request not found.";
            }

            return RedirectToAction(nameof(ViewPuppyRequests));
        }

        // A600: Register Puppies - GET
        [HttpGet]
        public async Task<IActionResult> RegisterPuppies(List<Puppy> puppies)
        {
            if (ModelState.IsValid)
            {
                var currentBreeder = await GetCurrentBreederAsync();
                if (currentBreeder == null) return RedirectToAction("Login", "Account");

                // Validate all puppies belong to the same litter and breeder owns it
                var litterIds = puppies.Select(p => p.LitterId).Distinct().ToList();
                if (litterIds.Count != 1)
                {
                    ModelState.AddModelError("", "All puppies must belong to the same litter.");
                    return View(puppies);
                }

                var litter = await _context.Litters
                .Include(l => l.BreedType)
                .FirstOrDefaultAsync(l => l.LitterId == litterIds[0] && l.BreederId == currentBreeder.BreederId);

                if (litter == null)
                {
                    ModelState.AddModelError("", "Invalid litter or access denied.");
                    return View(puppies);
                }

                // Validate puppy count matches litter specification
                if (puppies.Count != litter.NumPuppies)
                {
                    ModelState.AddModelError("", $"Must register exactly {litter.NumPuppies} puppies for this litter.");
                    return View(puppies);
                }

                foreach (var puppy in puppies)
                {
                    puppy.SaleStatus = SaleStatus.Available;
                    puppy.TrainingStatus = TrainingStatus.Untrained;
                    _context.Add(puppy);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Puppies have been successfully registered.";
                return RedirectToAction(nameof(Dashboard));
            }

            return View(puppies);
        }

        // A600: Register Puppies - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("RegisterPuppies")]
        public async Task<IActionResult> RegisterPuppiesPost(List<Puppy> puppies)
        {
            if (ModelState.IsValid)
            {
                var currentBreeder = await GetCurrentBreederAsync();
                if (currentBreeder == null) return RedirectToAction("Login", "Account");

                foreach (var puppy in puppies)
                {
                    // Verify the litter belongs to the current breeder
                    var litter = await _context.Litters
                        .FirstOrDefaultAsync(l => l.LitterId == puppy.LitterId && l.BreederId == currentBreeder.BreederId);

                    if (litter != null)
                    {
                        puppy.SaleStatus = SaleStatus.Available;
                        puppy.TrainingStatus = TrainingStatus.Untrained;
                        _context.Add(puppy);
                    }
                }
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Puppies have been successfully registered.";
                return RedirectToAction(nameof(Dashboard));
            }

            return View(puppies);
        }


        // A610: Remove Puppy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePuppy(int puppyId)
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var puppy = await _context.Puppies
                .Include(p => p.Litter)
                .Include(p => p.PuppyRequests)
                .FirstOrDefaultAsync(p => p.PuppyID == puppyId && p.Litter.BreederID == currentBreeder.BreederId);

            if (puppy == null)
            {
                TempData["ErrorMessage"] = "Puppy not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (puppy.SaleStatus == SaleStatus.Sold)
            {
                TempData["ErrorMessage"] = "Cannot remove a sold puppy.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (puppy.PuppyRequests.Any(pr => pr.Status == RequestStatus.Pending))
            {
                TempData["ErrorMessage"] = "Cannot remove puppy with pending requests.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Update litter puppy count
            puppy.Litter.NumPuppies = Math.Max(0, puppy.Litter.NumPuppies - 1);

            _context.Puppies.Remove(puppy);
            _context.Update(puppy.Litter);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Puppy {puppy.PuppyName} has been removed.";
            return RedirectToAction(nameof(Dashboard));
        }

        // A620: View Puppy Details
        public async Task<IActionResult> ViewPuppy(int? puppyId)
        {
            if (puppyId == null) return NotFound();

            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var puppy = await _context.Puppies
                .Include(p => p.Litter)
                .ThenInclude(l => l.BreedType)
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.PuppyId == puppyId && p.Litter.BreederId == currentBreeder.BreederId);

            if (puppy == null) return NotFound();

            return View(puppy);
        }

        // A700: Add BreedType to Specialization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBreedType(int breedId)
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var specialization = await _context.breedSpecializations
                .FirstOrDefaultAsync(bs => bs.BreederId == currentBreeder.BreederId && bs.BreedId == breedId);

            if (specialization != null)
            {
                // Reactivate if currently inactive
                if (specialization.Active == ActiveStatus.NO)
                {
                    specialization.Active = ActiveStatus.YES;
                    _context.Update(specialization);
                    TempData["SuccessMessage"] = "Breed specialization has been reactivated.";
                }
                else
                {
                    TempData["InfoMessage"] = "You already specialize in this breed.";
                }
            }
            else
            {
                // Create new specialization
                var newSpecialization = new BreedSpecialization
                {
                    BreederId = currentBreeder.BreederId,
                    BreedId = breedId,
                    Active = ActiveStatus.YES
                };
                _context.Add(newSpecialization);
                TempData["SuccessMessage"] = "Breed specialization has been added.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageBreedTypes));
        }

        // A710: Remove BreedType from Specialization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBreedType(int breedId)
        {
            var currentBreeder = await GetCurrentBreederAsync();
            if (currentBreeder == null) return RedirectToAction("Login", "Account");

            var specialization = await _context.breedSpecializations
                .FirstOrDefaultAsync(bs => bs.BreederId == currentBreeder.BreederId && bs.BreedId == breedId);

            if (specialization != null)
            {
                // Deactivate specialization (not deleted, just marked as inactive)
                specialization.Active = ActiveStatus.NO;
                _context.Update(specialization);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Breed has been removed from your specializations.";
            }
            else
            {
                TempData["ErrorMessage"] = "Specialization not found.";
            }

            return RedirectToAction(nameof(ManageBreedTypes));
        }

        /* Helper method to get the current authenticated breeder
        (Using the authenticated user's claims to retrieve breeder information)
        */
        private async Task<Breeder?> GetCurrentBreederAsync()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var breederId))
            {
                return null;
            }

            return await _context.Breeders
                .Include(b => b.BreedSpecializations)
                .ThenInclude(bs => bs.BreedType)
                .Include(b => b.Litters)
                .ThenInclude(l => l.BreedType)
                .Include(b => b.Litters)
                .ThenInclude(l => l.Puppies)
                .Include(b => b.PuppyRequests)
                .ThenInclude(pr => pr.Client)
                .Include(b => b.PuppyRequests)
                .ThenInclude(pr => pr.Puppy)
                .FirstOrDefaultAsync(b => b.BreederID == breederId);
        }
    }
}
