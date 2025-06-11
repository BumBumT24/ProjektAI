using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektCRUD20510.Models;
using ProjektCRUD20510.Repositories;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjektCRUD20510.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUzytkownikRepositorycs _uzytkownicy;

        public AccountController(IUzytkownikRepositorycs uzytkownicy)
        {
            _uzytkownicy = uzytkownicy;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string nazwa, string haslo, string returnUrl = null)
        {
            var user = _uzytkownicy.GetAll().FirstOrDefault(u => u.Nazwa == nazwa && u.Haslo == haslo);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Nazwa),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ? "Admin" : "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(Class model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_uzytkownicy.GetAll().Any(u => u.Nazwa == model.Nazwa))
                {
                    ModelState.AddModelError("Nazwa", "Ta nazwa użytkownika jest już zajęta.");
                    return View(model);
                }

                // Save the new user (assuming Add and SaveChangesAsync are implemented)
                _uzytkownicy.Add(model); // Ensure this method exists in your repository

                return RedirectToAction("Login");
            }
            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}