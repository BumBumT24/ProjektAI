using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektCRUD20510.Models;
using ProjektCRUD20510.Repositories;
using System.Diagnostics;
using System.Security.Claims;

namespace ProjektCRUD20510.Controllers
{
    [Authorize]
    public class WynikController(IWynikRepository repository, ClassManagerContext context) : Controller
    {
        private readonly IWynikRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly ClassManagerContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                // Admin can see all results
                return View(_repository.GetAll().ToList());
            }
            else
            {
                // Normal user can only see their own results
                int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
                return View(_repository.GetAll().Where(w => w.Id_Uzytkownika == currentUserId).ToList());
            }
        }

        public IActionResult Details(int id)
        {
            var wynik = _repository.Get(id);
            if (wynik == null)
            {
                Debug.WriteLine($"Details: Wynik with id={id} not found.");
                return NotFound();
            }

            if (!User.IsInRole("Admin"))
            {
                // Normal user can only view their own results
                int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
                if (wynik.Id_Uzytkownika != currentUserId)
                {
                    return Forbid();
                }
            }
            return View(wynik);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            try
            {
                if (_context.Testy == null || _context.Uzytkownik == null || _context.Kategorie == null)
                {
                    ModelState.AddModelError("", "Database tables not initialized.");
                    return View(new Wynik());
                }

                var tests = _context.Testy.ToList();
                var users = _context.Uzytkownik.ToList();
                var categories = _context.Kategorie.ToList();

                if (tests.Count == 0 || users.Count == 0 || categories.Count == 0)
                {
                    ModelState.AddModelError("", "Required data (tests, users, or categories) is missing. Please create them first.");
                    return View(new Wynik());
                }

                foreach (var test in tests)
                {
                    Debug.WriteLine($"Test Option: Id={test.Id}, NazwaTestu={test.NazwaTestu}");
                }
                foreach (var user in users)
                {
                    Debug.WriteLine($"User Option: Id={user.Id}, Nazwa={user.Nazwa}");
                }
                foreach (var category in categories)
                {
                    Debug.WriteLine($"Category Option: Id={category.Id}, Nazwa={category.Nazwa}");
                }

                ViewData["Id_Testu"] = new SelectList(tests, "Id", "NazwaTestu");
                ViewData["Id_Uzytkownika"] = new SelectList(users, "Id", "Nazwa");
                ViewData["Id_Kategorii"] = new SelectList(categories, "Id", "Nazwa");
                return View(new Wynik());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Create GET: {ex}");
                ModelState.AddModelError("", "An error occurred while loading the form.");
                return View(new Wynik());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Wynik newWynik)
        {
            try
            {
                if (newWynik == null)
                {
                    return BadRequest("Wynik model is null.");
                }

                Debug.WriteLine($"Form Data: Id_Testu={newWynik.Id_Testu}, Id_Uzytkownika={newWynik.Id_Uzytkownika}, Id_Kategorii={newWynik.Id_Kategorii}, wynik={newWynik.wynik}");

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
                    try
                    {
                        _repository.Add(newWynik);
                        Debug.WriteLine("Wynik added successfully.");
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception during save: {ex}");
                        if (ex.InnerException != null)
                        {
                            Debug.WriteLine($"Inner Exception: {ex.InnerException}");
                            Debug.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                        }
                        ModelState.AddModelError("", $"Error saving result: {ex.Message}");
                    }
                }

                var tests = _context.Testy.ToList();
                var users = _context.Uzytkownik.ToList();
                var categories = _context.Kategorie.ToList();
                ViewData["Id_Kategorii"] = new SelectList(categories, "Id", "Nazwa", newWynik.Id_Kategorii);
                ViewData["Id_Testu"] = new SelectList(tests, "Id", "NazwaTestu", newWynik.Id_Testu);
                ViewData["Id_Uzytkownika"] = new SelectList(users, "Id", "Nazwa", newWynik.Id_Uzytkownika);
                return View(newWynik);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Create POST: {ex}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException}");
                    Debug.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }
                ModelState.AddModelError("", $"Error saving result: {ex.Message}");
                ViewData["Id_Kategorii"] = new SelectList(_context.Kategorie.ToList(), "Id", "Nazwa", newWynik?.Id_Kategorii);
                ViewData["Id_Testu"] = new SelectList(_context.Testy.ToList(), "Id", "NazwaTestu", newWynik?.Id_Testu);
                ViewData["Id_Uzytkownika"] = new SelectList(_context.Uzytkownik.ToList(), "Id", "Nazwa", newWynik?.Id_Uzytkownika);
                return View(newWynik);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            Debug.WriteLine($"Attempting to retrieve Wynik with id={id}");
            var wynik = _repository.Get(id);
            if (wynik == null)
            {
                Debug.WriteLine($"Wynik with id={id} not found.");
                return NotFound();
            }
            Debug.WriteLine($"Wynik found: Id={wynik.Id}, Id_Testu={wynik.Id_Testu}, Id_Uzytkownika={wynik.Id_Uzytkownika}, Id_Kategorii={wynik.Id_Kategorii}, wynik={wynik.wynik}");

            var categories = _context.Kategorie.ToList();
            var tests = _context.Testy.ToList();
            var users = _context.Uzytkownik.ToList();
            Debug.WriteLine($"Categories count: {categories.Count}, Tests count: {tests.Count}, Users count: {users.Count}");

            ViewData["Id_Kategorii"] = new SelectList(categories, "Id", "Nazwa", wynik.Id_Kategorii);
            ViewData["Id_Testu"] = new SelectList(tests, "Id", "NazwaTestu", wynik.Id_Testu);
            ViewData["Id_Uzytkownika"] = new SelectList(users, "Id", "Nazwa", wynik.Id_Uzytkownika);
            return View(wynik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Wynik updatedWynik)
        {
            Debug.WriteLine($"POST Edit: id={id}, Id_Testu={updatedWynik.Id_Testu}, Id_Uzytkownika={updatedWynik.Id_Uzytkownika}, Id_Kategorii={updatedWynik.Id_Kategorii}, wynik={updatedWynik.wynik}");
            if (id != updatedWynik.Id)
            {
                Debug.WriteLine($"Id mismatch: Route id={id}, Model Id={updatedWynik.Id}");
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(id, updatedWynik);
                    Debug.WriteLine("Wynik updated successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception during update: {ex}");
                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine($"Inner Exception: {ex.InnerException}");
                        Debug.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                    }
                    ModelState.AddModelError("", $"Error updating result: {ex.Message}");
                }
            }
            else
            {
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        Debug.WriteLine($"ModelState Error: {error.Key} - {err.ErrorMessage}");
                    }
                }
            }
            var categories = _context.Kategorie.ToList();
            var tests = _context.Testy.ToList();
            var users = _context.Uzytkownik.ToList();
            ViewData["Id_Kategorii"] = new SelectList(categories, "Id", "Nazwa", updatedWynik.Id_Kategorii);
            ViewData["Id_Testu"] = new SelectList(tests, "Id", "NazwaTestu", updatedWynik.Id_Testu);
            ViewData["Id_Uzytkownika"] = new SelectList(users, "Id", "Nazwa", updatedWynik.Id_Uzytkownika);
            return View(updatedWynik);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var wynik = _repository.Get(id);
            if (wynik == null)
            {
                Debug.WriteLine($"Delete: Wynik with id={id} not found.");
                return NotFound();
            }
            return View(wynik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.Delete(id);
            Debug.WriteLine($"Wynik with id={id} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}