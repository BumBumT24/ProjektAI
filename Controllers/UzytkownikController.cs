using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektCRUD20510.Models;
using ProjektCRUD20510.Repositories;
using System.Security.Claims;

namespace ProjektCRUD20510.Controllers
{
    [Authorize]
    public class UzytkownikController : Controller
    {
        private readonly IUzytkownikRepositorycs _uzytkownicy;

        public UzytkownikController(IUzytkownikRepositorycs uzytkownicy)
        {
            _uzytkownicy = uzytkownicy;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                // Admin can see all users
                return View(_uzytkownicy.GetAll());
            }
            else
            {
                // Normal user can only see their own profile
                int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                return View(_uzytkownicy.GetAll().Where(u => u.Id == currentUserId));
                return View(_uzytkownicy.GetAll().Where(u => u.Id == currentUserId));
            }
        }

        [Authorize(Roles = "Admin")] // Only admins can create users
        public IActionResult Create()
        {
            return View(new Class());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Class uzytkownik)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _uzytkownicy.Add(uzytkownik);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating user: {ex.Message}");
                }
            }
            return View(uzytkownik);
        }

        public IActionResult Details(int id)
        {
            var uzytkownik = _uzytkownicy.Get(id);
            if (uzytkownik == null)
            {
                return NotFound();
            }

            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (!User.IsInRole("Admin") && currentUserId != id)
            {
                // Normal users can only view their own details
                return Forbid();
            }
            return View(uzytkownik);
        }

        
        public IActionResult Edit(int id)
        {
            var uzytkownik = _uzytkownicy.Get(id);
            if (uzytkownik == null)
            {
                return NotFound();
            }
            return View(uzytkownik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Class uzytkownik)
        {
            if (id != uzytkownik.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _uzytkownicy.Update(id, uzytkownik);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating user: {ex.Message}");
                }
            }
            return View(uzytkownik);
        }

        [Authorize(Roles = "Admin")] // Only admins can delete users
        public IActionResult Delete(int id)
        {
            var uzytkownik = _uzytkownicy.Get(id);
            if (uzytkownik == null)
            {
                return NotFound();
            }
            return View(uzytkownik);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _uzytkownicy.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting user: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}