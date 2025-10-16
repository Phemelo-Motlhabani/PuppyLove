using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PupV1.Data;
using PupV1.Models;

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
        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManger, SignInManager<ApplicationUser> signInManager)
        { _context = context;
            _userManager = userManger;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult RegisterAsTrainer()
        {
            return View();
        }
        /*[HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.username);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.password, model.rememberMe, lockoutOnFailure:false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
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
        }*/
        [HttpPost]
        public async Task<IActionResult> RegisterAsTrainer(TrainerRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = await
                    _context.Trainers.FirstOrDefaultAsync(c => c.Username == model.username);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Username has been taken");
                    return View(model);
                }

                var trainer = new Trainer
                {
                    Username = model.username,
                    Password = model.Password,
                    Fname = model.Fname,
                    Lname = model.Lname,
                    Suburb = model.suburb,
                    City = model.city,
                    CellNum = model.CellNum,
                    Email = model.Email,
                    //TrainerId = model.TrainerId,
                };
                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();
                //Identity user
                var identityUser = new ApplicationUser
                {
                    UserName = model.username,
                    Email = model.Email ?? "",
                    Name = model.Fname ?? "",
                    Surname = model.Lname ?? "",
                    Suburb = model.suburb ?? "",
                    City = model.city ?? "",
                    CellNUm = model.CellNum ?? "",
                    TrainerId = trainer.TrainerId




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

                await _userManager.AddToRoleAsync(identityUser, "Trainer");
                /*var trainer = new Trainer
                {
                    Username = model.username,
                    Password = model.Password,
                    Fname = model.Fname,
                    Lname = model.Lname,
                    Suburb = model.suburb,
                    City = model.city,
                    CellNum = model.CellNum,
                    Email = model.Email,
                    //TrainerId = model.TrainerId,
                };
                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();*/


                await _signInManager.SignInAsync(identityUser, isPersistent: false);
                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);

        }
        [HttpGet]
        public IActionResult RegisterAsClient()
        {
            return View();
        }
        /*[HttpGet]
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
                    return RedirectToAction("Index", "Dashboard");
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
        }*/
        [HttpPost]
        public async Task<IActionResult> RegisterAsClient(ClientRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = await
                    _context.Clients.FirstOrDefaultAsync(c => c.Username == model.Username);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Username has been taken");
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
                var identityUser = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email ?? "",
                    Name = model.Fname ?? "",
                    Surname = model.Lname ?? "",
                    Suburb = model.Suburb ?? "",
                    City = model.City?? "",
                    CellNUm = model.CellNum ?? "",
                    ClientId = client.ClientId
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

                await _userManager.AddToRoleAsync(identityUser, "Client");
                await _signInManager.SignInAsync(identityUser, isPersistent: false);
                return RedirectToAction("Index", "ClientDashboard");

            }
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterAsBreeder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsBreeder(BreederRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = await
                    _context.Breeders.FirstOrDefaultAsync(c => c.Username == model.Username);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Username has been taken");
                    return View(model);
                }

                var breeder = new Breeder
                {
                    Username = model.Username,
                    Password = model.Password,
                    Fname = model.Fname,
                    Lname = model.Lname,
                    Suburb = model.Suburb,
                    City = model.City,
                    CellNum = model.CellNum,
                    Email = model.Email,

                };
                _context.Breeders.Add(breeder);
                await _context.SaveChangesAsync();
                //Identity user
                var identityUser = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email ?? "",
                    Name = model.Fname ?? "",
                    Surname = model.Lname ?? "",
                    Suburb = model.Suburb ?? "",
                    City = model.City ?? "",
                    CellNUm = model.CellNum ?? "",
                    BreederId = breeder.BreederId
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

                await _userManager.AddToRoleAsync(identityUser, "Breeder");
                await _signInManager.SignInAsync(identityUser, isPersistent: false);
                return RedirectToAction("Index", "BreederDashboard");

            }
            return View(model);
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

