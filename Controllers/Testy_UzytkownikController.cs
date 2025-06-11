using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektCRUD20510.Models;
using ProjektCRUD20510.Repositories;
using System.Diagnostics;

namespace ProjektCRUD20510.Controllers
{
    public class Testy_UzytkownikController : Controller
    {
        private readonly ITestyUzytkownikRepository _repository;
        private readonly Models.ClassManagerContext _context;

        public Testy_UzytkownikController(ITestyUzytkownikRepository repository, Models.ClassManagerContext context)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            return View(_repository.GetAll());
        }

        public IActionResult Details(int id_testu, int id_uzytkownika)
        {
            var testyUzytkownik = _repository.Get(id_testu, id_uzytkownika);
            if (testyUzytkownik == null)
            {
                return NotFound();
            }
            return View(testyUzytkownik);
        }

        public IActionResult Create()
        {
            try
            {
                if (_context.Testy == null || _context.Uzytkownik == null)
                {
                    ModelState.AddModelError("", "Database tables not initialized.");
                    return View(new Testy_Uzytkownik());
                }

                var tests = _context.Testy.ToList();
                var users = _context.Uzytkownik.ToList();

                if (!tests.Any() || !users.Any())
                {
                    ModelState.AddModelError("", "No tests or users available. Please create them first.");
                    return View(new Testy_Uzytkownik());
                }

                ViewBag.Id_Testu = new SelectList(tests, "Id", "NazwaTestu");
                ViewBag.Id_Uzytkownika = new SelectList(users, "Id", "Nazwa");

                return View(new Testy_Uzytkownik());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Create GET: {ex}");
                ModelState.AddModelError("", "An error occurred while loading the form.");
                return View(new Testy_Uzytkownik());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Testy_Uzytkownik testyUzytkownik)
        {
            try
            {
                Debug.WriteLine($"Form Data: Id_Testu={testyUzytkownik.Id_Testu}, Id_Uzytkownika={testyUzytkownik.Id_Uzytkownika}");

                if (testyUzytkownik == null)
                {
                    return BadRequest("Testy_Uzytkownik model is null.");
                }

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState)
                    {
                        foreach (var err in error.Value.Errors)
                        {
                            Debug.WriteLine($"ModelState Error: {error.Key} - {err.ErrorMessage}");
                        }
                    }
                    ModelState.AddModelError("", "Please correct the errors in the form.");
                }

                if (ModelState.IsValid)
                {
                    _repository.Add(testyUzytkownik);
                    Debug.WriteLine("Testy_Uzytkownik added successfully.");
                    return RedirectToAction(nameof(Index));
                }

                // Repopulate dropdowns if validation fails
                ViewBag.Id_Testu = new SelectList(_context.Testy, "Id", "NazwaTestu", testyUzytkownik.Id_Testu);
                ViewBag.Id_Uzytkownika = new SelectList(_context.Uzytkownik, "Id", "Nazwa", testyUzytkownik.Id_Uzytkownika);
                return View(testyUzytkownik);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Create POST: {ex}");
                ModelState.AddModelError("", $"An error occurred while saving: {ex.Message}");

                // Repopulate dropdowns
                ViewBag.Id_Testu = new SelectList(_context.Testy, "Id", "NazwaTestu", testyUzytkownik?.Id_Testu);
                ViewBag.Id_Uzytkownika = new SelectList(_context.Uzytkownik, "Id", "Nazwa", testyUzytkownik?.Id_Uzytkownika);
                return View(testyUzytkownik);
            }
        }

        public IActionResult Edit(int id_testu, int id_uzytkownika)
        {
            var testyUzytkownik = _repository.Get(id_testu, id_uzytkownika);
            if (testyUzytkownik == null)
            {
                return NotFound();
            }
            ViewData["Id_Testu"] = new SelectList(_context.Testy, "Id", "NazwaTestu", testyUzytkownik.Id_Testu);
            ViewData["Id_Uzytkownika"] = new SelectList(_context.Uzytkownik, "Id", "Nazwa", testyUzytkownik.Id_Uzytkownika);
            return View(testyUzytkownik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id_testu, int id_uzytkownika, Testy_Uzytkownik testyUzytkownik)
        {
            if (id_testu != testyUzytkownik.Id_Testu || id_uzytkownika != testyUzytkownik.Id_Uzytkownika)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _repository.Update(id_testu, id_uzytkownika, testyUzytkownik);
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Testu"] = new SelectList(_context.Testy, "Id", "NazwaTestu", testyUzytkownik.Id_Testu);
            ViewData["Id_Uzytkownika"] = new SelectList(_context.Uzytkownik, "Id", "Nazwa", testyUzytkownik.Id_Uzytkownika);
            return View(testyUzytkownik);
        }

        public IActionResult Delete(int id_testu, int id_uzytkownika)
        {
            var testyUzytkownik = _repository.Get(id_testu, id_uzytkownika);
            if (testyUzytkownik == null)
            {
                return NotFound();
            }
            return View(testyUzytkownik);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id_testu, int id_uzytkownika)
        {
            _repository.Delete(id_testu, id_uzytkownika);
            return RedirectToAction(nameof(Index));
        }
    }
}