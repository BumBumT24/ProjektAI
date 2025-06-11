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
    public class FiszkiController : Controller
    {
        private readonly IFiszkiRepository _repository;
        private readonly ClassManagerContext _context;

        public FiszkiController(IFiszkiRepository repository, ClassManagerContext context)
        {
            _repository = repository;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_repository.GetAll().ToList());
        }

        public IActionResult Details(int id)
        {
            var fiszki = _repository.Get(id);
            if (fiszki == null)
            {
                return NotFound();
            }
            return View(fiszki);
        }

        public IActionResult Create()
        {
            var users = _context.Uzytkownik.ToList();
            if (!users.Any())
            {
                ModelState.AddModelError("", "No users available. Please create a user first.");
                return View(new Fiszki());
            }

            var selectList = new SelectList(users, "Id", "Nazwa");
            foreach (var item in selectList)
            {
                System.Diagnostics.Debug.WriteLine($"SelectList Item: Value={item.Value}, Text={item.Text}");
            }
            ViewData["UzytkownikId"] = selectList;
            return View(new Fiszki());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Fiszki fiszki)
        {
            if (fiszki == null)
            {
                return BadRequest("Fiszki model is null.");
            }

            // Automatically set Id_Uzytkownika to the current user's ID
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
            fiszki.Id_Uzytkownika = currentUserId;

            // Debug: Log form data
            System.Diagnostics.Debug.WriteLine($"Form Data: Id_Uzytkownika={fiszki.Id_Uzytkownika}, Nazwa_PL={fiszki.Nazwa_PL}, Nazwa_ENGLISH={fiszki.Nazwa_ENGLISH}");

            if (!ModelState.IsValid)
            {
                // Debug: Log ModelState errors
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"ModelState Error: {error.Key} - {err.ErrorMessage}");
                    }
                }
                ModelState.AddModelError("", "Please correct the errors in the form.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Add(fiszki);
                    System.Diagnostics.Debug.WriteLine("Record added successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", $"Error saving flashcard: {ex.Message}");
                }
            }

            // Reload SelectList for re-rendering
            ViewData["UzytkownikId"] = new SelectList(_context.Uzytkownik, "Id", "Nazwa", fiszki.Id_Uzytkownika);
            return View(fiszki);
        }

        public IActionResult Edit(int id)
        {
            var fiszki = _repository.Get(id);
            if (fiszki == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner or an admin
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
            if (!User.IsInRole("Admin") && fiszki.Id_Uzytkownika != currentUserId)
            {
                return Forbid();
            }

            ViewData["UzytkownikId"] = new SelectList(_context.Uzytkownik, "Id", "Nazwa", fiszki.Id_Uzytkownika);
            return View(fiszki);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Fiszki fiszki)
        {
            if (id != fiszki.Id)
            {
                return NotFound();
            }

            // Check if the current user is the owner or an admin
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
            if (!User.IsInRole("Admin") && fiszki.Id_Uzytkownika != currentUserId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(id, fiszki);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating flashcard: {ex.Message}");
                }
            }
            ViewData["UzytkownikId"] = new SelectList(_context.Uzytkownik, "Id", "Nazwa", fiszki.Id_Uzytkownika);
            return View(fiszki);
        }

        public IActionResult Delete(int id)
        {
            var fiszki = _repository.Get(id);
            if (fiszki == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner or an admin
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
            if (!User.IsInRole("Admin") && fiszki.Id_Uzytkownika != currentUserId)
            {
                return Forbid();
            }

            return View(fiszki);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var fiszki = _repository.Get(id);
            if (fiszki == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner or an admin
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
            if (!User.IsInRole("Admin") && fiszki.Id_Uzytkownika != currentUserId)
            {
                return Forbid();
            }

            _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}