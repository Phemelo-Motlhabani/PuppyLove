using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PupV1.Data;
using PupV1.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace PupV1.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var trainer = await _context.Trainers
                .Include(t => t.Trainerskills)
                .ThenInclude(ts => ts.Skill)
                .Include(t => t.Trainingrequests)
                .ThenInclude(r => r.Client)
               .FirstOrDefaultAsync(t => t.Username == user.UserName);
           if (trainer == null) return NotFound();

            var viewModel = new DashboardViewModel
            {
                Name = trainer.Fname,
                Surname = trainer.Lname,
                Suburb = trainer.Suburb,
                City = trainer.City,
                CellNUm = trainer.CellNum,
                //TrainerID = trainer.TrainerId,
                Username = trainer.Username,
                ImageFile = trainer.ImageFile,

                SelectedSkills = trainer.Trainerskills.Select(ts => new TrainerSkillDisplayViewModel
                {
                    SkillName = ts.Skill.SkillName,
                    SkillLevel = ts.SkillLevel
                }).ToList(),

                TrainingRequests = await _context.Trainingrequests
                .Include(r => r.Client)
                .Where(r => r.TrainerId == trainer.TrainerId)
                .ToListAsync()

            };
            viewModel.TrainingProgresses = await _context.TrainingProgresses
     .Where(p => p.TrainerId == trainer.TrainerId)
     .ToListAsync();
            /*if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new DashboardViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Suburb = user.Suburb,
                City = user.City,
                CellNUm = user.CellNUm,
                TrainerID = user.TrainerID,
               // Email = user.Email,
                //UserName = user.UserName,
                //Password = user.Password,
            };*/
            Console.WriteLine($"*** Dashboard Index: Found {viewModel.TrainingProgresses.Count} training progresses ***");
            TempData["IndexDebug"] = $"Found {viewModel.TrainingProgresses.Count} training records in database";
            return View(viewModel);
        }
       /* public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync (User);
            var skills = _context.Trainerskills
                .Include(us => us.Skill)
                .Where(us => us.TrainerId == user.TrainerID)
                .ToList();

            return View(skills);
        }*/
        public async Task<IActionResult> EditSkills()
        {
            var user = await _userManager.GetUserAsync(User);
            var allSkills = await _context.Skills.ToListAsync();

            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Username == user.UserName);
            if (trainer == null) return NotFound();
            var trainerSkills = await _context.Trainerskills
                .Where(ts => ts.TrainerId == trainer.TrainerId)
                .ToListAsync();

            var viewModel = new TrainerSkillsFormViewModel
            {
                Skills = allSkills.Select(skill => new SkillSelectionViewModel
                {
                    SkillId = skill.SkillId,
                    SkillName = skill.SkillName,
                    IsSelected = trainerSkills.Any(ts => ts.SkillId == skill.SkillId),
                    SkillLevel = trainerSkills.FirstOrDefault(ts => ts.SkillId == skill.SkillId)?.SkillLevel ?? "Beginner"
                }).ToList()
            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile(DashboardViewModel model)
        {
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Username == model.Username);
            if (trainer == null) return NotFound();

            trainer.Fname = model.Name;
            trainer.Lname = model.Surname;
            trainer.Suburb = model.Suburb;
            trainer.City = model.City;
            trainer.CellNum = model.CellNUm;
            trainer.ImageFile = model.ImageFile;

            await _context.SaveChangesAsync();
            TempData["Message"] = "Profile updated!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSkills(TrainerSkillsFormViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Username == user.UserName);
            if (trainer == null) return NotFound();

            var existingSkills = _context.Trainerskills
                .Where(ts => ts.TrainerId == trainer.TrainerId);

            _context.Trainerskills.RemoveRange(existingSkills);

            foreach (var skill in model.Skills)
            {
                if (skill.IsSelected)
                {
                    _context.Trainerskills.Add(new Trainerskill
                    {
                        TrainerId = trainer.TrainerId,
                        SkillId = skill.SkillId,
                        SkillLevel = skill.SkillLevel,
                    });
                }
            }
            await _context.SaveChangesAsync();
            TempData["Message"] = "Skills updated successfully!";
            return RedirectToAction("Index");
        }
       /* [HttpGet]
        public async Task<IActionResult> TrainingRequests()
        {
            var user = await _userManager.GetUserAsync(User);

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(t => t.Username == user.UserName);
            if (trainer == null) return NotFound();

            var requests = await _context.Trainingrequests
                .Include(r => r.Client)
                .Where(r => r.TrainerId == trainer.TrainerId)
                .ToListAsync();
            return View(requests);
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>AcceptRequest(string id)
        {
            var request = await _context.Trainingrequests.FindAsync(id);
            if (request == null) return NotFound();

            request.IsAccepted = true;
            request.RequestStatus = "Accepted";

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(string id)
        {
            var request = await _context.Trainingrequests.FindAsync(id);
            if (request == null) return NotFound();

            request.IsAccepted = false;
            request.RequestStatus = "Rejected";

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProgress(TrainingProgress progress)
        {
            TempData["Debug"] = "Step 1: Method called";
            Console.WriteLine("*** Step 1: AddProgress method started ***");

            // Check if we received data
            Console.WriteLine($"Received: DogName='{progress.DogName}', DogBreed='{progress.DogBreed}', OwnerName='{progress.OwnerName}', Program='{progress.Program}'");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Debug"] = "Step 2: User is null!";
                return RedirectToAction("Index");
            }

            TempData["Debug"] = $"Step 2: User found - {user.UserName}";
            Console.WriteLine($"*** Step 2: User found: {user.UserName} ***");

            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Username == user.UserName);
            if (trainer == null)
            {
                TempData["Debug"] = "Step 3: Trainer not found!";
                return RedirectToAction("Index");
            }

            TempData["Debug"] = $"Step 3: Trainer found - ID: {trainer.TrainerId}";
            Console.WriteLine($"*** Step 3: Trainer found: {trainer.TrainerId} ***");

            // Check ModelState
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                TempData["Debug"] = $"Step 4: ModelState invalid - {errors}";
                return RedirectToAction("Index");
            }

            TempData["Debug"] = "Step 4: ModelState is valid";
            Console.WriteLine("*** Step 4: ModelState is valid ***");

            try
            {
                // Set trainer ID
                progress.TrainerId = trainer.TrainerId;

                TempData["Debug"] = "Step 5: About to add to context";
                Console.WriteLine("*** Step 5: Adding to context ***");

                _context.TrainingProgresses.Add(progress);

                TempData["Debug"] = "Step 6: About to save changes";
                Console.WriteLine("*** Step 6: Saving changes ***");

                var result = await _context.SaveChangesAsync();

                TempData["Debug"] = $"Step 7: SUCCESS! Saved {result} records";
                Console.WriteLine($"*** Step 7: SUCCESS! SaveChangesAsync returned: {result} ***");

                TempData["Message"] = "Training added successfully!";
            }
            catch (Exception ex)
            {
                TempData["Debug"] = $"Step ERROR: {ex.Message}";
                Console.WriteLine($"*** ERROR: {ex.Message} ***");
                Console.WriteLine($"*** Stack Trace: {ex.StackTrace} ***");
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
        /*public async Task<IActionResult> AddProgress(TrainingProgress progress)
        {
            var user = await _userManager.GetUserAsync(User);
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Username == user.UserName);
            if (trainer == null) return NotFound();

            Console.WriteLine($"Received: DogName={progress.DogName}, DogBreed={progress.DogBreed}, OwnerName={progress.OwnerName}, Program={progress.Program}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new {Field = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage)}).ToList();
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error - Field: {error.Field}, Errors: {string.Join(", ", error.Errors)}");
                }

                TempData["Error"] = "Please fill in all required fields correctly.";
                return RedirectToAction("Index");
                
                
            }
            try
            {
                progress.TrainerId = trainer.TrainerId;
                _context.TrainingProgresses.Add(progress);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Training progress added successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                TempData["Error"] = $"Error saving to database: {ex.Message}";
            }
            return RedirectToAction("Index");
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(TrainingProgress progress)
        {
            Console.WriteLine($"*** UpdateProgress called with ProgressId: {progress.ProgressId} ***");
            Console.WriteLine($"*** New ProgressNotes: '{progress.ProgressNotes}' ***");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                TempData["Error"] = $"Update failed - {errors}";
                Console.WriteLine($"*** UpdateProgress ModelState invalid: {errors} ***");
                return RedirectToAction("Index");
            }

            try
            {
                var existing = await _context.TrainingProgresses.FindAsync(progress.ProgressId);
                if (existing != null)
                {
                    Console.WriteLine($"*** Found existing record: {existing.DogName} ***");
                    Console.WriteLine($"*** Old notes: '{existing.ProgressNotes}' ***");

                    existing.ProgressNotes = progress.ProgressNotes;
                    existing.IsFinished = progress.IsFinished;

                    Console.WriteLine($"*** Updated notes: '{existing.ProgressNotes}' ***");

                    var result = await _context.SaveChangesAsync();
                    Console.WriteLine($"*** SaveChanges result: {result} ***");

                    TempData["Message"] = "Progress notes updated successfully!";
                }
                else
                {
                    Console.WriteLine($"*** No record found with ProgressId: {progress.ProgressId} ***");
                    TempData["Error"] = "Training record not found!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** UpdateProgress Error: {ex.Message} ***");
                TempData["Error"] = $"Update failed: {ex.Message}";
            }

            return RedirectToAction("Index");
            /*if (ModelState.IsValid)
            {
                var existing = await _context.TrainingProgresses.FindAsync(progress.ProgressId);
                if (existing != null)
                {
                    existing.ProgressNotes = progress.ProgressNotes;
                    existing.IsFinished = progress.IsFinished;

                    await _context.SaveChangesAsync();
                }
                    /*_context.Update(progress);
                await _context.SaveChangesAsync();*/
            /*}
            //return RedirectToAction("Index");*/
        }
        [HttpPost]
        public async Task<IActionResult> FinishTraining(int id)
        {
            var progress = await _context.TrainingProgresses.FindAsync(id);
            if (progress != null)
            {
                progress.IsFinished = true;
                _context.Update(progress);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        
    }

}
