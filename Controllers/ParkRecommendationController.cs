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
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "uploads", "parks");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await park.ImageFile.CopyToAsync(fileStream);
                    }

                    park.ImageUrl = "/images/uploads/parks/" + fileName;
                    
                }

                _context.Add(park);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Dashboard");
            }
            return View(park);
        }
       
        public IActionResult Create()
        {
            return View();
        }
    }
}
