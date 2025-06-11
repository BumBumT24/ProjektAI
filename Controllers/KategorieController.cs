using Microsoft.AspNetCore.Mvc;
using ProjektCRUD20510.Models;
using ProjektCRUD20510.Repositories;

namespace ProjektCRUD20510.Controllers
{
    public class KategorieController : Controller
    {
        private readonly IKategorieRepository _repository;

        public KategorieController(IKategorieRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.GetAll());
        }

        public IActionResult Details(int id)
        {
            var kategorie = _repository.Get(id);
            if (kategorie == null)
            {
                return NotFound();
            }
            return View(kategorie);
        }

        public IActionResult Create()
        {
            return View(new Kategorie());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Kategorie kategorie)
        {
            if (ModelState.IsValid)
            {
                _repository.Add(kategorie);
                return RedirectToAction(nameof(Index));
            }
            return View(kategorie);
        }

        public IActionResult Edit(int id)
        {
            var kategorie = _repository.Get(id);
            if (kategorie == null)
            {
                return NotFound();
            }
            return View(kategorie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Kategorie kategorie)
        {
            if (id != kategorie.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _repository.Update(id, kategorie);
                return RedirectToAction(nameof(Index));
            }
            return View(kategorie);
        }

        public IActionResult Delete(int id)
        {
            var kategorie = _repository.Get(id);
            if (kategorie == null)
            {
                return NotFound();
            }
            return View(kategorie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}