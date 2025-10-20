using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PupV1.Data;
using PupV1.Models;
using System.Linq.Expressions;

namespace PupV1.Controllers
{

    public class AccountController : Controller
    {
        public IActionResult ChooseRole()
        {
            return View();
        }
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManger, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult RegisterAsTrainer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsTrainer(TrainerRegisterViewModel model)
        {
            if (ModelState.IsValid)
                return View(model);
            {
                //var existing = await _context.Trainers.FirstOrDefaultAsync(c => c.Username == model.username);
                var existing = await _userManager.FindByNameAsync(model.username);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Username has been taken");
                    return View(model);
                }

                var identityUser = new ApplicationUser
                {
                    UserName = model.username,
                    Name = model.Fname,
                    Surname = model.Lname,
                    Suburb = model.suburb,
                    City = model.city,
                    CellNUm = model.CellNum,
                    Email = model.Email,
                    //TrainerId = model.TrainerId,
                };
                var result = await _userManager.CreateAsync(identityUser, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                //Identity user
                var trainer = new Trainer
                {
                    Username = model.username,
                    Email = model.Email ?? "",
                    Fname = model.Fname ?? "",
                    Lname = model.Lname ?? "",
                    Suburb = model.suburb ?? "",
                    City = model.city ?? "",
                    CellNum = model.CellNum ?? "",
                    //TrainerId = trainer.TrainerId




                };
                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();

                identityUser.TrainerId = trainer.TrainerId;
                await _userManager.UpdateAsync(identityUser);

                await _userManager.AddToRoleAsync(identityUser, "Trainer");


                await _signInManager.SignInAsync(identityUser, isPersistent: false);
                return RedirectToAction("Index", "Dashboard");
            }


        }
        [HttpGet]
        public IActionResult RegisterAsClient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsClient(ClientRegisterViewModel model)
        {
            if (ModelState.IsValid)
                return View(model);
            {
                //var existing = await _context.Clients.FirstOrDefaultAsync(c => c.Username == model.Username);
                var existing = await _userManager.FindByNameAsync(model.Username);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Username has been taken");
                    return View(model);
                }
                var identityUser = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email ?? "",
                    Name = model.Fname ?? "",
                    Surname = model.Lname ?? "",
                    Suburb = model.Suburb ?? "",
                    City = model.City ?? "",
                    CellNUm = model.CellNum ?? "",
                    //ClientId = client.ClientId
                };

                var result = await _userManager.CreateAsync(identityUser, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                var client = new Client
                {
                    Username = model.Username,
                    Password = model.Password,
                    Fname = model.Fname,
                    Lname = model.Lname,
                    Suburb = model.Suburb,
                    City = model.City,
                    CellNum = model.CellNum,
                    Email = model.Email,
                    PostCode = model.PostCode

                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                //Identity user
                identityUser.ClientId = client.ClientId;
                await _userManager.UpdateAsync(identityUser);

                await _userManager.AddToRoleAsync(identityUser, "Client");
                await _signInManager.SignInAsync(identityUser, isPersistent: false);
                return RedirectToAction("Index", "ClientDashboard");

            }

        }

        [HttpGet]
        public IActionResult RegisterAsBreeder()
        {
            Console.WriteLine("GET RegisterAsBreeder called");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsBreeder(BreederRegisterViewModel model)
        {
            Console.WriteLine("==================== REGISTER ACTION HIT ====================");
            Console.WriteLine($"Username: {model.Username}");
            Console.WriteLine($"Email: {model.Email}");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(model);
            }
            try
            {


                // var existing = await _context.Breeders.FirstOrDefaultAsync(c => c.Username == model.Username);
                var existing = await _userManager.FindByNameAsync(model.Username);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Username has been taken");
                    return View(model);
                }

                string? imageUrl = null;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    imageUrl = "/image/profiles/" + uniqueFileName;
                }
                else
                {
                    imageUrl = "/images/default-profile.jpg";
                }
                var identityUser = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email ?? "",
                    Name = model.Fname ?? "",
                    Surname = model.Lname ?? "",
                    Suburb = model.Suburb ?? "",
                    City = model.City ?? "",
                    CellNUm = model.CellNum ?? "",
                    KennelName = model.KennelName ?? "",
                    //BreederId = breeder.BreederId
                };

                var result = await _userManager.CreateAsync(identityUser, model.Password);
                if (!result.Succeeded)
                {
                    Console.WriteLine("Identity User creation failed");
                    if (imageUrl != null && imageUrl != "/images/default-profile.jpg")
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Identity Error: {error.Code} - {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                Console.WriteLine("Identity User Created Successfully!");
                Console.WriteLine("Creating Breeder Record...");
                var breeder = new Breeder
                {
                    Username = model.Username,
                    //Password = model.Password,
                    Fname = model.Fname,
                    Lname = model.Lname,
                    Suburb = model.Suburb,
                    City = model.City,
                    CellNum = model.CellNum,
                    Email = model.Email,
                    KennelName = model.KennelName,
                    LicenceNum = model.LicenceNum,
                    ImageUrl = imageUrl,

                };
                _context.Breeders.Add(breeder);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Breeder Record Created! BreederId: {breeder.BreederId}");
                //Identity user
                identityUser.BreederId = breeder.BreederId;
                await _userManager.UpdateAsync(identityUser);
                Console.WriteLine("Identity User Linked to Breeder!");

                // Check if Breeder role exists
                if (!await _context.Roles.AnyAsync(r => r.Name == "Breeder"))
                {
                    Console.WriteLine("ERROR: Breeder role does not exist in database!");
                    ModelState.AddModelError("", "Breeder role is not configured. Please contact administrator.");
                    return View(model);
                }
                await _userManager.AddToRoleAsync(identityUser, "Breeder");
                await _signInManager.SignInAsync(identityUser, isPersistent: false);
                Console.WriteLine("User Signed In! Redirecting to Dashboard...");
                return RedirectToAction("Index", "BreederDashboard");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"An error occurred during registration: {ex.Message}");
                return View(model);
            }
            
            
         
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.username);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.password, model.rememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    Console.WriteLine($"User {user.UserName} has roles: {string.Join(",",roles)}");
                    if(roles.Contains("Trainer"))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else if(roles.Contains("Client"))
                    {
                        return RedirectToAction("Index", "ClientDashboard");
                    }
                    else if(roles.Contains("Breeder"))
                    {
                        return RedirectToAction("Index", "BreederDashboard");
                    }
                    Console.WriteLine("No matching role found, redirecting to Home");
                    return RedirectToAction("Index", "Home");
                }

            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}   

