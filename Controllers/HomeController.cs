using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektCRUD20510.Models;
using ProjektCRUD20510.Models.ViewModels;
using ProjektCRUD20510.Repositories;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace ProjektCRUD20510.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClassManagerContext _context;
        private readonly IFiszkiRepository _fiszkiRepository;

        public HomeController(ILogger<HomeController> logger, ClassManagerContext context, IFiszkiRepository fiszkiRepository)
        {
            _logger = logger;
            _context = context;
            _fiszkiRepository = fiszkiRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FiszkiSetup()
        {
            return View(new FiszkiSetupViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FiszkiSetup(FiszkiSetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Fetch all Fiszki from the database
            var fiszki = _fiszkiRepository.GetAll().ToList();

            if (!fiszki.Any())
            {
                ModelState.AddModelError("", "No flashcards available. Please create some flashcards first.");
                return View(model);
            }

            // Shuffle and take the requested number of Fiszki
            var random = new Random();
            var selectedFiszki = fiszki.OrderBy(x => random.Next())
                                       .Take(model.NumberOfFiszki)
                                       .ToList();

            // Store the selected Fiszki and direction in session
            HttpContext.Session.SetString("FiszkiList", JsonSerializer.Serialize(selectedFiszki));
            HttpContext.Session.SetString("Direction", model.Direction);
            HttpContext.Session.SetInt32("CurrentIndex", 0);

            return RedirectToAction("FiszkiPractice");
        }

        public IActionResult FiszkiPractice()
        {
            // Retrieve the Fiszki list, direction, and current index from session
            var fiszkiJson = HttpContext.Session.GetString("FiszkiList");
            var direction = HttpContext.Session.GetString("Direction");
            var currentIndex = HttpContext.Session.GetInt32("CurrentIndex") ?? 0;

            if (string.IsNullOrEmpty(fiszkiJson) || string.IsNullOrEmpty(direction))
            {
                return RedirectToAction("FiszkiSetup");
            }

            var fiszkiList = JsonSerializer.Deserialize<List<Fiszki>>(fiszkiJson);
            if (fiszkiList == null || currentIndex >= fiszkiList.Count)
            {
                // Clear session when done
                HttpContext.Session.Remove("FiszkiList");
                HttpContext.Session.Remove("Direction");
                HttpContext.Session.Remove("CurrentIndex");
                return RedirectToAction("Index");
            }

            var viewModel = new FiszkiPracticeViewModel
            {
                CurrentFiszka = fiszkiList[currentIndex],
                Direction = direction,
                CurrentIndex = currentIndex + 1,
                TotalCount = fiszkiList.Count
            };

            return View(viewModel);
        }

        public IActionResult NextFiszka()
        {
            var currentIndex = HttpContext.Session.GetInt32("CurrentIndex") ?? 0;
            HttpContext.Session.SetInt32("CurrentIndex", currentIndex + 1);
            return RedirectToAction("FiszkiPractice");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}