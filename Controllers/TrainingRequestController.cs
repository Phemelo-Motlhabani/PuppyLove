using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    public class TrainingRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Trainer> _userManager;

        public TrainingRequestController(ApplicationDbContext context, UserManager<Trainer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       [Authorize]
       public async Task<IActionResult> MyRequests()
        {
            var trainer = await _userManager.GetUserAsync(User);

            var requests = await _context.Trainingrequests
                .Include(r => r.Client)
                .Where(r => r.TrainerId == trainer.TrainerId)
                .ToListAsync();

            return View(requests);
        }
        [HttpPost]
        public async Task<IActionResult>AcceptRequest(string id)
        {
            var request = await _context.Trainingrequests.FindAsync(id);
            if (request == null) return NotFound();

            request.RequestStatus = "Accepted";
            await _context.SaveChangesAsync();
            /*_context.Update(request);

            var progress = new TrainingProgress
            {
                TrequestId = request.TrequestId,
                TrainerId = request.TrainerId,
                ProgressPercentage = 0,
                Notes = "Training started",
                StartDate = DateTime.Now,
            };

            _context.TrainingProgresses.Add(progress);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");*/

            return RedirectToAction("TrainingRequests");
        }
        [HttpPost]
        public async Task<IActionResult>RejectRequest(string id)
        {
            var request = await _context.Trainingrequests.FindAsync(id);
            if (request == null) return NotFound();

            request.RequestStatus = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction("TrainingRequests");
        }

       /* public IActionResult Index()
        {
            return View();
        }*/
    }
}
