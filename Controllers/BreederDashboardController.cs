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
    [Authorize(Roles = "Breeder")]
    public class BreederDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BreederDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.BreederId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var breeder = await _context.Breeders.FirstOrDefaultAsync(b => b.BreederId == user.BreederId);
            if (breeder == null)
            {
                return NotFound();
            }
            var viewModel = new BreederDashboardViewModel
            {
                Username = breeder.Username,
                Name = breeder.Fname,
                Surname = breeder.Lname,
                City = breeder.City,
                CellNUm = breeder.CellNum,
                KennelName = breeder.KennelName,
                ImageUrl = breeder.ImageUrl ?? "/images/default-profile.jpg"
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(BreederDashboardViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);

            if(user?.BreederId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var breeder = await _context.Breeders.FirstOrDefaultAsync(b => b.BreederId == user.BreederId);
            if (breeder == null) return NotFound();

            breeder.Fname = model.Name;
            breeder.Lname = model.Surname;
            breeder.Suburb = model.Suburb;
            breeder.City = model.City;
            breeder.CellNum = model.CellNUm;
            breeder.KennelName = model.KennelName;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, breeder.ImageUrl.TrimStart('/'));
                if(!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
                if (!string.IsNullOrEmpty(breeder.ImageUrl) && breeder.ImageUrl != "/images/default-profile.jpg")
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, breeder.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }
                breeder.ImageUrl = "/images/profiles/" + uniqueFileName;
            }
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Suburb = model.Suburb;
            user.City = model.City;
            user.CellNUm = model.CellNUm;
            user.KennelName = model.KennelName;

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Profile updated!";
            return RedirectToAction("Index");
        }
    }  
}
