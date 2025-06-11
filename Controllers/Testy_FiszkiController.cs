using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektCRUD20510.Models;
using ProjektCRUD20510.Repositories;
using System.Diagnostics;

namespace ProjektCRUD20510.Controllers
{
    public class Testy_FiszkiController(ITestyFiszkiRepository repository, ClassManagerContext context) : Controller
    {
        private readonly ITestyFiszkiRepository _repository = repository;
        private readonly ClassManagerContext _context = context;

        public IActionResult Index()
        {
            return View(_repository.GetAll());
        }

        public IActionResult Details(int id_testu, int id_fiszki)
        {
            var testyFiszki = _repository.Get(id_testu, id_fiszki);
            if (testyFiszki == null)
            {
                Debug.WriteLine($"Details: Testy_Fiszki with Id_Testu={id_testu}, Id_Fiszki={id_fiszki} not found.");
                return NotFound();
            }
            return View(testyFiszki);
        }

        public IActionResult Create()
        {
            var tests = _context.Testy.ToList();
            var fiszki = _context.Fiszki.ToList();
            Debug.WriteLine($"Create GET: Tests count: {tests.Count}, Fiszki count: {fiszki.Count}");
            if (tests.Count == 0 || fiszki.Count == 0)
            {
                ModelState.AddModelError("", "No tests or fiszki available. Please create them first.");
            }
            ViewData["Id_Fiszki"] = new SelectList(fiszki, "Id", "Nazwa_ENGLISH");
            ViewData["Id_Testu"] = new SelectList(tests, "Id", "NazwaTestu");
            return View(new Testy_Fiszki());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Testy_Fiszki testy_Fiszki)
        {
            Debug.WriteLine($"Create POST: Id_Testu={testy_Fiszki.Id_Testu}, Id_Fiszki={testy_Fiszki.Id_Fiszki}");
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
                    _repository.Add(testy_Fiszki);
                    Debug.WriteLine("Testy_Fiszki added successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception during Add: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    ModelState.AddModelError("", $"Error saving Testy_Fiszki: {ex.Message}");
                }
            }
            var tests = _context.Testy.ToList();
            var fiszki = _context.Fiszki.ToList();
            ViewData["Id_Fiszki"] = new SelectList(fiszki, "Id", "Nazwa_ENGLISH", testy_Fiszki.Id_Fiszki);
            ViewData["Id_Testu"] = new SelectList(tests, "Id", "NazwaTestu", testy_Fiszki.Id_Testu);
            return View(testy_Fiszki);
        }

        public IActionResult Edit(int id_testu, int id_fiszki)
        {
            var testyFiszki = _repository.Get(id_testu, id_fiszki);
            if (testyFiszki == null)
            {
                Debug.WriteLine($"Edit GET: Testy_Fiszki with Id_Testu={id_testu}, Id_Fiszki={id_fiszki} not found.");
                return NotFound();
            }
            var tests = _context.Testy.ToList();
            var fiszki = _context.Fiszki.ToList();
            Debug.WriteLine($"Edit GET: Tests count: {tests.Count}, Fiszki count: {fiszki.Count}");
            ViewData["Id_Fiszki"] = new SelectList(fiszki, "Id", "Nazwa_ENGLISH", testyFiszki.Id_Fiszki);
            ViewData["Id_Testu"] = new SelectList(tests, "Id", "NazwaTestu", testyFiszki.Id_Testu);
            return View(testyFiszki);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id_testu, int id_fiszki, Testy_Fiszki testy_Fiszki)
        {
            Debug.WriteLine($"Edit POST: Id_Testu={testy_Fiszki.Id_Testu}, Id_Fiszki={testy_Fiszki.Id_Fiszki}");
            if (id_testu != testy_Fiszki.Id_Testu || id_fiszki != testy_Fiszki.Id_Fiszki)
            {
                Debug.WriteLine($"Edit POST: Id mismatch. Route: Id_Testu={id_testu}, Id_Fiszki={id_fiszki}; Model: Id_Testu={testy_Fiszki.Id_Testu}, Id_Fiszki={testy_Fiszki.Id_Fiszki}");
                return NotFound();
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
                try
                {
                    _repository.Update(id_testu, id_fiszki, testy_Fiszki);
                    Debug.WriteLine("Testy_Fiszki updated successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception during Update: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    ModelState.AddModelError("", $"Error updating Testy_Fiszki: {ex.Message}");
                }
            }
            var tests = _context.Testy.ToList();
            var fiszki = _context.Fiszki.ToList();
            ViewData["Id_Fiszki"] = new SelectList(fiszki, "Id", "Nazwa_ENGLISH", testy_Fiszki.Id_Fiszki);
            ViewData["Id_Testu"] = new SelectList(tests, "Id", "NazwaTestu", testy_Fiszki.Id_Testu);
            return View(testy_Fiszki);
        }

        public IActionResult Delete(int id_testu, int id_fiszki)
        {
            var testyFiszki = _repository.Get(id_testu, id_fiszki);
            if (testyFiszki == null)
            {
                Debug.WriteLine($"Delete GET: Testy_Fiszki with Id_Testu={id_testu}, Id_Fiszki={id_fiszki} not found.");
                return NotFound();
            }
            return View(testyFiszki);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id_testu, int id_fiszki)
        {
            try
            {
                _repository.Delete(id_testu, id_fiszki);
                Debug.WriteLine($"Testy_Fiszki with Id_Testu={id_testu}, Id_Fiszki={id_fiszki} deleted successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during Delete: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError("", $"Error deleting Testy_Fiszki: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}