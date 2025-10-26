using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClientDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.ClientId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == user.ClientId);
            if (client == null)
            {
                return NotFound();
            }
            var viewModel = new ClientDashboardViewModel
            {
                Username = client.Username,
                Fname = client.Fname,
                Lname = client.Lname,
                City = client.City,
                CellNum = client.CellNum,
                Suburb = client.Suburb,
                ImageUrl = client.ImageUrl ?? "/images/default-profile.jpg"
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ClientDashboardViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user?.ClientId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == user.ClientId);
            if (client == null) return NotFound();

            client.Fname = model.Fname;
            client.Lname = model.Lname;
            client.Suburb = model.Suburb;
            client.City = model.City;
            client.CellNum = model.CellNum;
            

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath,"images", "profiles");
                //if (!Directory.Exists(uploadsFolder))
                
                    Directory.CreateDirectory(uploadsFolder);
                
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
                if (!string.IsNullOrEmpty(client.ImageUrl) && client.ImageUrl != "/images/default-profile.jpg")
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, client.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }
                client.ImageUrl = "/images/profiles/" + uniqueFileName;
            }
            user.Name = model.Fname;
            user.Surname = model.Lname;
            user.Suburb = model.Suburb;
            user.City = model.City;
            user.CellNUm = model.CellNum;
           

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Profile updated!";
            return RedirectToAction("Index");
        }
    }
}

