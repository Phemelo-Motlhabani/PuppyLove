using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    public class PuppyRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PuppyRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Client")]
        [HttpGet]
        public async Task<IActionResult> BrowsePuppies()
        {
            var availablePuppies = await _context.Puppies
                .Include(p => p.Litter)
                .ThenInclude(l => l.BreedType)
                .Include(p => p.Litter)
                .ThenInclude(l => l.Breeder)
                .Where(p => p.Status == "Available" || string.IsNullOrEmpty(p.Status))
                . OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return View(availablePuppies);
        }

        [Authorize(Roles = "Client")]
        [HttpGet]
        public async Task<IActionResult> RequestPuppy(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if(user?.ClientId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var puppy = await _context.Puppies
                .Include(p => p.Litter)
                    .ThenInclude(l => l.BreedType)
                .FirstOrDefaultAsync(p => p.PuppyId == id);

            if (puppy == null || puppy.Status != "Available")
            {
                TempData["ErrorMessage"] = "This puppy is not available anymore.";
                return RedirectToAction("BrowsePuppies");
            }
            var existingRequest = await _context.Puppyrequests
                .FirstOrDefaultAsync(r => r.PuppyId == id &&
                                         r.ClientId == user.ClientId &&
                                         r.Status == "Pending");

            if (existingRequest != null)
            {
                TempData["ErrorMessage"] = "You already have a pending request for this puppy.";
                return RedirectToAction("MyRequests");
            }
            var model = new PuppyRequestViewModel
            {
                PuppyId = puppy.PuppyId,
                PuppyName = puppy.PuppyName ?? "Unnamed",
                BreedName = puppy.Litter?.BreedType?.BreedName ?? "Unknown",
                Price = puppy.Price,
                ImageUrl = puppy.ImageUrl
            };

            return View(model);
        }
        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPuppy(PuppyRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user?.ClientId == null)
                return RedirectToAction("Login", "Account");

            var puppy = await _context.Puppies
                .Include(p => p.Litter)
                .FirstOrDefaultAsync(p => p.PuppyId == model.PuppyId);

            if (puppy == null || puppy.Status != "Available")
            {
                TempData["ErrorMessage"] = "This puppy is not available anymore.";
                return RedirectToAction("BrowsePuppies");
            }

            var request = new Puppyrequest
            {
                PuppyId = model.PuppyId,
                ClientId = user.ClientId.Value,
                BreederId = puppy.Litter?.BreederId,
                Status = "Pending",
                Message = model.Message,
                RequestDate = DateTime.Now
            };

            _context.Puppyrequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Your request for {puppy.PuppyName} has been sent to the breeder!";
            return RedirectToAction("MyRequests");
        }
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MyRequests()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.ClientId == null)
                return RedirectToAction("Login", "Account");

            var requests = await _context.Puppyrequests
                .Include(r => r.Puppy)
                    .ThenInclude(p => p.Litter)
                        .ThenInclude(l => l.BreedType)
                .Include(r => r.Breeder)
                .Where(r => r.ClientId == user.ClientId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }
        [Authorize(Roles = "Breeder")]
        public async Task<IActionResult> IncomingRequests()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
                return RedirectToAction("Login", "Account");

            var requests = await _context.Puppyrequests
                .Include(r => r.Puppy)
                    .ThenInclude(p => p.Litter)
                        .ThenInclude(l => l.BreedType)
                .Include(r => r.Client)
                .Where(r => r.BreederId == user.BreederId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }
        [Authorize(Roles = "Breeder")]
        public async Task<IActionResult> RequestDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
                return RedirectToAction("Login", "Account");

            var request = await _context.Puppyrequests
                .Include(r => r.Puppy)
                    .ThenInclude(p => p.Litter)
                        .ThenInclude(l => l.BreedType)
                .Include(r => r.Client)
                .Include(r => r.Breeder)
                .FirstOrDefaultAsync(r => r.RequestId == id && r.BreederId == user.BreederId);

            if (request == null)
                return NotFound();

            return View(request);
        }
        [Authorize(Roles = "Breeder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RespondToRequest(int requestId, string status, string? breederResponse)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
                return RedirectToAction("Login", "Account");

            var request = await _context.Puppyrequests
                .Include(r => r.Puppy)
                .FirstOrDefaultAsync(r => r.RequestId == requestId && r.BreederId == user.BreederId);

            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
            {
                TempData["ErrorMessage"] = "This request has already been processed.";
                return RedirectToAction("IncomingRequests");
            }

            request.Status = status; 
            request.BreederResponse = breederResponse;
            request.ResponseDate = DateTime.Now;

            if (status == "Accepted" && request.Puppy != null)
            {
                request.Puppy.Status = "Reserved";
                request.Puppy.ClientId = request.ClientId;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Request has been {status.ToLower()}.";
            return RedirectToAction("IncomingRequests");
        }
        [Authorize(Roles = "Breeder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsSold(int requestId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.BreederId == null)
                return RedirectToAction("Login", "Account");

            var request = await _context.Puppyrequests
                .Include(r => r.Puppy)
                    .ThenInclude(p => p.Litter)
                .FirstOrDefaultAsync(r => r.RequestId == requestId && r.BreederId == user.BreederId);

            if (request == null)
                return NotFound();

            if (request.Status != "Accepted")
            {
                TempData["ErrorMessage"] = "Can only mark accepted requests as sold.";
                return RedirectToAction("IncomingRequests");
            }

            if (request.Puppy != null)
            {
                request.Puppy.Status = "Sold";
                request.Puppy.SoldDate = DateTime.Now;

                if (request.Puppy.Litter != null && request.Puppy.Litter.AvailablePuppies > 0)
                {
                    request.Puppy.Litter.AvailablePuppies--;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Puppy marked as sold!";
            return RedirectToAction("IncomingRequests");
        }
        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.ClientId == null)
                return RedirectToAction("Login", "Account");

            var request = await _context.Puppyrequests
                .FirstOrDefaultAsync(r => r.RequestId == id && r.ClientId == user.ClientId);

            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Can only cancel pending requests.";
                return RedirectToAction("MyRequests");
            }

            _context.Puppyrequests.Remove(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Request cancelled.";
            return RedirectToAction("MyRequests");
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
