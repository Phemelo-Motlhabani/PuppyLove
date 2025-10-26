using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    public class TrainingRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainingRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> SendRequest(int trainerId)
        {
            var trainer = await _context.Trainers.FindAsync(trainerId);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction("BrowseTrainers", "Review");
            }

            var breeds = await _context.Breedtypes
                .Select(b => new { b.BreedName })
                .ToListAsync();

            ViewBag.Breeds = new SelectList(breeds, "BreedName", "BreedName");

            var model = new TrainingRequestViewModel
            {
                TrainerId = trainer.TrainerId,
                TrainerName = $"{trainer.Fname} {trainer.Lname}",
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest(TrainingRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Breeds = new SelectList(_context.Breedtypes, "BreedName", "BreedName");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == user.Email);

            if (client == null)
            {
                TempData["ErrorMessage"] = "Client not found";
                return RedirectToAction("BrowseTrainers", "Review");
            }

            var exists = await _context.Trainingrequests
                .AnyAsync(r => r.ClientId == client.ClientId && r.TrainerId == model.TrainerId && r.RequestStatus == "Pending");

            if (exists)
            {
                TempData["ErrorMessage"] = "You already have a pending request with this trainer";
                return RedirectToAction("BrowseTrainers", "Review");
            }

            if (model.TrainerId == 0)
            {
                TempData["ErrorMessage"] = "Trainer ID missing — request not sent.";
                return View(model);
            }

            var request = new Trainingrequest
            {
                ClientId = client.ClientId,
                TrainerId = model.TrainerId,
                DogName = model.DogName,
                DogBreed = model.DogBreed,
                TrainingProgram = model.TrainingProgram,
                AdditionalInfo = model.AdditionalInfo,
                RequestDate = DateTime.Now,
                RequestStatus = "Pending",
                IsAccepted = false
            };

            _context.Trainingrequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Training request sent successfully!";
            return RedirectToAction("BrowseTrainers", "Review");
        }

        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> MyRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == user.Email);

            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var requests = await _context.Trainingrequests
                .Include(r => r.Client)
                .Where(r => r.TrainerId == trainer.TrainerId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            
            var activeSessions = await _context.TrainingProgresses
                .Where(p => p.TrainerId == trainer.TrainerId && !p.IsFinished)
                .CountAsync();

            var completedSessions = await _context.TrainingProgresses
                .Where(p => p.TrainerId == trainer.TrainerId && p.IsFinished)
                .CountAsync();

            ViewBag.ActiveSessions = activeSessions;
            ViewBag.CompletedSessions = completedSessions;

            return View(requests);
        }

        [HttpPost]
        [Authorize(Roles = "Trainer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(int id, string status)
        {
            Console.WriteLine($"Respond called: id={id}, status={status}");

            var request = await _context.Trainingrequests
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.TrequestId == id);

            if (request == null)
            {
                TempData["ErrorMessage"] = "Request not found.";
                return RedirectToAction("MyRequests");
            }

            if (status != "Accepted" && status != "Declined")
            {
                TempData["ErrorMessage"] = "Invalid status.";
                return RedirectToAction("MyRequests");
            }

            if (status == "Accepted")
            {
                var trainer = await _context.Trainers
                    .FirstOrDefaultAsync(t => t.TrainerId == request.TrainerId);

                if (trainer == null)
                {
                    TempData["ErrorMessage"] = "Trainer not found.";
                    return RedirectToAction("MyRequests");
                }

                var progress = new TrainingProgress
                {
                    ClientId = request.ClientId,
                    TrequestId = request.TrequestId,
                    DogName = request.DogName,
                    DogBreed = request.DogBreed,
                    OwnerName = $"{request.Client?.Fname} {request.Client?.Lname}",
                    TrainerId = trainer.TrainerId,
                    Program = request.TrainingProgram,
                    ProgressNotes = "Training started - Initial session scheduled.",
                    IsFinished = false
                };
                _context.TrainingProgresses.Add(progress);
            }

            
            _context.Trainingrequests.Remove(request);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = status == "Accepted"
                ? "Request accepted and training started."
                : "Request declined.";

            return RedirectToAction("MyRequests");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}