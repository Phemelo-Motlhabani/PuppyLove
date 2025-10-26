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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("==================== Dashboard Index Called ====================");

            var user = await _userManager.GetUserAsync(User);
            Console.WriteLine($"User: {user?.UserName}");

            var trainer = await _context.Trainers
                .Include(t => t.Trainerskills)
                .ThenInclude(ts => ts.Skill)
                .FirstOrDefaultAsync(t => t.Username == user.UserName);

            if (trainer == null)
            {
                Console.WriteLine("ERROR: Trainer not found!");
                return NotFound();
            }

            Console.WriteLine($"Trainer found: {trainer.Fname} {trainer.Lname} (ID: {trainer.TrainerId})");

            
            var trainingRequests = await _context.Trainingrequests
                .Include(r => r.Client)
                .Where(r => r.TrainerId == trainer.TrainerId && r.RequestStatus == "Pending")
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            Console.WriteLine($"Found {trainingRequests.Count} pending training requests");
            foreach (var req in trainingRequests)
            {
                Console.WriteLine($"  - {req.DogName} ({req.RequestStatus})");
            }

            
            var trainingProgresses = await _context.TrainingProgresses
                .Where(p => p.TrainerId == trainer.TrainerId)
                .OrderByDescending(p => p.IsFinished ? 0 : 1) 
                .ThenByDescending(p => p.ProgressId)
                .ToListAsync();

            Console.WriteLine($"Found {trainingProgresses.Count} training progress records");
            foreach (var prog in trainingProgresses)
            {
                Console.WriteLine($"  - {prog.DogName} (Finished: {prog.IsFinished})");
            }

            var viewModel = new DashboardViewModel
            {
                Name = trainer.Fname,
                Surname = trainer.Lname,
                Suburb = trainer.Suburb,
                City = trainer.City,
                CellNUm = trainer.CellNum,
                Username = trainer.Username,
                ImageUrl = trainer.ImageUrl,
                ImageFile = trainer.ImageFile,

                SelectedSkills = trainer.Trainerskills.Select(ts => new TrainerSkillDisplayViewModel
                {
                    SkillName = ts.Skill.SkillName,
                    SkillLevel = ts.SkillLevel
                }).ToList(),

                TrainingRequests = trainingRequests,
                TrainingProgresses = trainingProgresses
            };

            Console.WriteLine("==================== Dashboard Index Complete ====================");
            TempData["IndexDebug"] = $"Requests: {trainingRequests.Count}, Progress: {trainingProgresses.Count}";

            return View(viewModel);
            
        }

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
            //trainer.ImageFile = model.ImageFile;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

               
                if (!string.IsNullOrEmpty(trainer.ImageUrl) && trainer.ImageUrl != "/images/default-profile.jpg")
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, trainer.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                
                trainer.ImageUrl = "/images/profiles/" + uniqueFileName;
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            Console.WriteLine($"==================== AcceptRequest Called ====================");
            Console.WriteLine($"Request ID: {id}");

            var request = await _context.Trainingrequests
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.TrequestId == id);

            if (request == null)
            {
                Console.WriteLine("ERROR: Request not found!");
                TempData["ErrorMessage"] = "Training request not found.";
                return RedirectToAction("Index");
            }

            Console.WriteLine($"Request Found: {request.DogName}");

            try
            {
                
                var progress = new TrainingProgress
                {
                    ClientId = request.ClientId,
                    TrequestId = request.TrequestId,
                    DogName = request.DogName,
                    DogBreed = request.DogBreed,
                    OwnerName = $"{request.Client?.Fname} {request.Client?.Lname}",
                    TrainerId = request.TrainerId,
                    Program = request.TrainingProgram,
                    ProgressNotes = "Training started - Initial session scheduled.",
                    IsFinished = false
                };

                Console.WriteLine("Adding TrainingProgress to context...");
                _context.TrainingProgresses.Add(progress);

                Console.WriteLine("Saving TrainingProgress FIRST...");
                var result1 = await _context.SaveChangesAsync();
                Console.WriteLine($"TrainingProgress saved: {result1} rows affected");

                
                Console.WriteLine("Now removing Trainingrequest...");
                _context.Trainingrequests.Remove(request);

                Console.WriteLine("Saving removal...");
                var result2 = await _context.SaveChangesAsync();
                Console.WriteLine($"Request removal saved: {result2} rows affected");

                if (result1 > 0 && result2 > 0)
                {
                    Console.WriteLine("SUCCESS: Both operations completed!");
                    TempData["SuccessMessage"] = $"Request accepted and training started for {request.DogName}!";
                }
                else
                {
                    Console.WriteLine($"WARNING: result1={result1}, result2={result2}");
                    TempData["ErrorMessage"] = "Something went wrong with the save operation.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR OCCURRED: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                TempData["ErrorMessage"] = $"Error accepting request: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int id)
        {
            Console.WriteLine($"==================== RejectRequest Called ====================");
            Console.WriteLine($"Request ID: {id}");

            var request = await _context.Trainingrequests.FindAsync(id);

            if (request == null)
            {
                Console.WriteLine("ERROR: Request not found!");
                TempData["ErrorMessage"] = "Training request not found.";
                return RedirectToAction("Index");
            }

            Console.WriteLine($"Rejecting request for: {request.DogName}");

            try
            {
               
                _context.Trainingrequests.Remove(request);

                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChanges result: {result} rows affected");

                TempData["SuccessMessage"] = "Training request declined.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                TempData["ErrorMessage"] = $"Error rejecting request: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProgress(TrainingProgress progress)
        {
            TempData["Debug"] = "Step 1: Method called";
            Console.WriteLine("*** Step 1: AddProgress method started ***");

            
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(int ProgressId, string ProgressNotes, bool IsFinished)
        {
            Console.WriteLine($"*** UpdateProgress called with ProgressId: {ProgressId} ***");
            Console.WriteLine($"*** New ProgressNotes: '{ProgressNotes}' ***");
            Console.WriteLine($"*** IsFinished: {IsFinished} ***");

            try
            {
                var existing = await _context.TrainingProgresses.FindAsync(ProgressId);

                if (existing == null)
                {
                    Console.WriteLine($"*** No record found with ProgressId: {ProgressId} ***");
                    TempData["ErrorMessage"] = "Training record not found!";
                    return RedirectToAction("Index");
                }

                Console.WriteLine($"*** Found existing record: {existing.DogName} ***");
                Console.WriteLine($"*** Old notes: '{existing.ProgressNotes}' ***");

                
                existing.ProgressNotes = ProgressNotes;
                

                Console.WriteLine($"*** Updated notes: '{existing.ProgressNotes}' ***");

                
                _context.Entry(existing).State = EntityState.Modified;

                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"*** SaveChanges result: {result} rows affected ***");

                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Progress notes updated successfully!";
                }
                else
                {
                    Console.WriteLine("*** WARNING: No rows affected! ***");
                    TempData["ErrorMessage"] = "No changes were saved.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** UpdateProgress Error: {ex.Message} ***");
                Console.WriteLine($"*** Stack Trace: {ex.StackTrace} ***");
                TempData["ErrorMessage"] = $"Update failed: {ex.Message}";
            }

            return RedirectToAction("Index");
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
        
    


