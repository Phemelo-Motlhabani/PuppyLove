using Microsoft.AspNetCore.Mvc;
using PupV1.Data;
using PupV1.Models;

namespace PupV1.Controllers
{
    public class TrainingProgressController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingProgressController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TrainingProgress progress)
        {
            if(ModelState.IsValid)
            {
                _context.Add(progress);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(progress);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TrainingProgress progress)
        {
            if (id != progress.ProgressId) return NotFound();

            if(ModelState.IsValid)
            {
                _context.Update(progress);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(progress);
        }

        [HttpPost]
        public IActionResult Finish(int id)
        {
            var progress = _context.TrainingProgresses.Find(id);
            if (progress == null) return NotFound();

            progress.IsFinished=true;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            return View(_context.TrainingProgresses.ToList());
        }
    }
}
