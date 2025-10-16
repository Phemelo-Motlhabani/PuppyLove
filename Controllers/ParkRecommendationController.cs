using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    [Authorize(Roles ="Trainer")]
    public class ParkRecommendationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParkRecommendationController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }
       /* [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Parkrecommendation park)
        {
            if (!ModelState.IsValid)
            {
                return View(park);
            }

            // ✅ Get the logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // ✅ Get the corresponding Trainer from the database
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == user.Email);
            if (trainer == null)
            {
                ModelState.AddModelError("", "Trainer not found. Please register as a trainer.");
                return View(park);
            }

            // ✅ Assign the correct TrainerID
            park.TrainerId = trainer.TrainerId;

            // ✅ Handle image upload
            if (park.ImageFile != null)
            {
                string wwwRootPath = _env.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(park.ImageFile.FileName);
                string path = Path.Combine(wwwRootPath, "uploads/parks/", fileName);

                using (var filestream = new FileStream(path, FileMode.Create))
                {
                    await park.ImageFile.CopyToAsync(filestream);
                }

                park.ImageUrl = "/uploads/parks/" + fileName;
            }

            // ✅ Save record with valid TrainerID
            _context.Add(park);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult>Create(Parkrecommendation park)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (!user.TrainerId.HasValue)
                {
                    var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == user.Email);
                    if (trainer == null)
                    {
                        TempData["Error"] = "Trainer profile not found.";
                        return RedirectToAction("Index", "Dashboard");
                    }
                    park.TrainerId = trainer.TrainerId;
                }
                else
                {
                    park.TrainerId = user.TrainerId.Value;
                }
                if (park.ImageFile != null)
                {
                    string wwwRootPath = _env.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(park.ImageFile.FileName);
                    string uploadsFolder = Path.Combine(wwwRootPath, "uploads/parks/", fileName);

                    if(!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string path=Path.Combine(uploadsFolder, fileName);
                    using (var filestream = new FileStream(path, FileMode.Create))
                    {
                        await park.ImageFile.CopyToAsync(filestream);
                    }
                    park.ImageUrl = "/uploads/parks/" + fileName;
                }

                _context.Add(park);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Dashboard");
            }
            return View(park);
        }
       // [Authorize(Roles = "Trainer")]
        public IActionResult Create()
        {
            return View();
        }
    }
}
