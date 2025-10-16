using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    [Authorize]
    public class SkillController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SkillController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> SelectSkills()
        {
            var skills = await _context.Skills.ToListAsync();

            var viewModel = skills.Select(s => new SkillSelectionViewModel
            {
                SkillId = s.SkillId,
                SkillName = s.SkillName,
            }).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SelectSkills(List<SkillSelectionViewModel> model)
        {
            var user = await _userManager.GetUserAsync(User);
            int TrainerId = user.TrainerId ??0;

            foreach (var skill in model)
            {
                var existing = await _context.Trainerskills
                    .FirstOrDefaultAsync(ts => ts.TrainerId == TrainerId && ts.SkillId == skill.SkillId);
                if (skill.IsSelected)
                {
                    if (existing == null)
                    {
                        _context.Trainerskills.Add(new Trainerskill
                        {
                            TrainerId = TrainerId,
                            SkillId = skill.SkillId,
                            SkillLevel = skill.SkillLevel,
                        });
                    }
                    else
                    {
                        existing.SkillLevel = skill.SkillLevel;
                    }
                }
                else if (existing != null)
                {
                    _context.Trainerskills.Remove(existing);
                }
            }

                /*var selectedSkills = model.Where(m=> m.IsSelected).ToList();

                foreach (var skill in selectedSkills)
                {
                    var userSkill = new Trainerskill
                    {
                        TrainerId = user.TrainerId,
                        SkillId = skill.SkillId,
                        SkillLevel = skill.SkillLevel,
                    };
                    _context.Trainerskills.Add(userSkill);
                }
                */
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Dashboard");
               
            }
            /*[HttpPost]
            public async Task<IActionResult>Delete(int? id)
            {
                var user = await _userManager.GetUserAsync (User);
                var trainerSkill = await _context.Trainerskills.FirstOrDefaultAsync(ts => ts.TrainerId == user.TrainerId && ts.SkillId == id);

                if (trainerSkill == null)
                {
                    return NotFound();
                }

                _context.Trainerskills.Remove(trainerSkill);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Dashboard");

            public IActionResult Index()
            {
                return View();
            }*/
        }
    }

