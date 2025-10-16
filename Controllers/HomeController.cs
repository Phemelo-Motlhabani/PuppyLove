using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PupV1.Data;
using PupV1.Models;
using System.Diagnostics;

namespace PupV1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        //public HomeController(ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

       /* public IActionResult Index()
        {
            return View();
        }*/

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult>Index()
        {
            var parks = await _context.Parkrecommendations.ToListAsync();
            return View(parks);
        }
    }
}
