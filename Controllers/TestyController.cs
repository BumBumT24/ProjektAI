using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektCRUD20510.Models;
using ProjektCRUD20510.Models.ViewModels;
using ProjektCRUD20510.Repositories;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace ProjektCRUD20510.Controllers
{
    [Authorize]
    public class TestyController : Controller
    {
        private readonly ITestyRepository _repository;
        private readonly ClassManagerContext _context;

        public TestyController(ITestyRepository repository, ClassManagerContext context)
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
            var testy = _repository.Get(id);
            if (testy == null)
            {
                Debug.WriteLine($"Details: Testy with id={id} not found.");
                return NotFound();
            }
            return View(testy);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Id_Kategorii = new SelectList(_context.Kategorie, "Id", "Nazwa");
            return View(new Testy());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Testy testy)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Id_Kategorii = new SelectList(_context.Kategorie, "Id", "Nazwa", testy.Id_Kategorii);
                return View(testy);
            }

            try
            {
                _repository.Add(testy);
                Debug.WriteLine("Testy added successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during save: {ex}");
                ModelState.AddModelError("", $"Error saving test: {ex.Message}");
                ViewBag.Id_Kategorii = new SelectList(_context.Kategorie, "Id", "Nazwa", testy.Id_Kategorii);
                return View(testy);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var testy = _repository.Get(id);
            if (testy == null)
            {
                Debug.WriteLine($"Edit: Testy with id={id} not found.");
                return NotFound();
            }

            var viewModel = new TestyEditViewModel
            {
                Id = testy.Id,
                Id_Kategorii = testy.Id_Kategorii,
                NazwaTestu = testy.NazwaTestu,
                SelectedFiszkiIds = testy.Testy_Fiszki.Select(tf => tf.Id_Fiszki).ToList()
            };

            ViewBag.Id_Kategorii = new SelectList(_context.Kategorie, "Id", "Nazwa", testy.Id_Kategorii);
            ViewBag.Fiszki = _context.Fiszki.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, TestyEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                Debug.WriteLine($"Id mismatch: Route id={id}, Model Id={viewModel.Id}");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Id_Kategorii = new SelectList(_context.Kategorie, "Id", "Nazwa", viewModel.Id_Kategorii);
                ViewBag.Fiszki = _context.Fiszki.ToList();
                return View(viewModel);
            }

            try
            {
                var testy = new Testy
                {
                    Id = viewModel.Id,
                    Id_Kategorii = viewModel.Id_Kategorii,
                    NazwaTestu = viewModel.NazwaTestu,
                    Testy_Fiszki = viewModel.SelectedFiszkiIds.Select(fiszkaId => new Testy_Fiszki
                    {
                        Id_Testu = viewModel.Id,
                        Id_Fiszki = fiszkaId
                    }).ToList()
                };

                _repository.Update(id, testy);
                Debug.WriteLine("Testy updated successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during update: {ex}");
                ModelState.AddModelError("", $"Error updating test: {ex.Message}");
                ViewBag.Id_Kategorii = new SelectList(_context.Kategorie, "Id", "Nazwa", viewModel.Id_Kategorii);
                ViewBag.Fiszki = _context.Fiszki.ToList();
                return View(viewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var testy = _repository.Get(id);
            if (testy == null)
            {
                Debug.WriteLine($"Delete: Testy with id={id} not found.");
                return NotFound();
            }
            return View(testy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.Delete(id);
            Debug.WriteLine($"Testy with id={id} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult TakeTest(int id)
        {
            var test = _repository.Get(id);
            if (test == null)
            {
                Debug.WriteLine($"TakeTest: Test with id={id} not found.");
                return NotFound();
            }

            return View(new TestSetupViewModel { TestId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TakeTest(TestSetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var test = _repository.Get(model.TestId);
            if (test == null)
            {
                Debug.WriteLine($"TakeTest: Test with id={model.TestId} not found.");
                return NotFound();
            }

            var fiszki = test.Testy_Fiszki.Select(tf => _context.Fiszki.Find(tf.Id_Fiszki)).Where(f => f != null).ToList();
            var random = new Random();
            var shuffledFiszki = fiszki.OrderBy(x => random.Next()).ToList();

            HttpContext.Session.SetString("TestFiszki", JsonSerializer.Serialize(shuffledFiszki));
            HttpContext.Session.SetString("TestDirection", model.Direction);
            HttpContext.Session.SetInt32("TestCurrentIndex", 0);
            HttpContext.Session.SetString("TestAnswers", JsonSerializer.Serialize(new Dictionary<int, string>()));
            HttpContext.Session.SetInt32("TestId", model.TestId); // Store TestId in session

            return RedirectToAction("TestQuestion");
        }

        public IActionResult TestQuestion()
        {
            var fiszkiJson = HttpContext.Session.GetString("TestFiszki");
            var direction = HttpContext.Session.GetString("TestDirection");
            var currentIndex = HttpContext.Session.GetInt32("TestCurrentIndex") ?? 0;
            var answersJson = HttpContext.Session.GetString("TestAnswers");

            if (string.IsNullOrEmpty(fiszkiJson) || string.IsNullOrEmpty(direction))
            {
                return RedirectToAction("Index");
            }

            var fiszkiList = JsonSerializer.Deserialize<List<Fiszki>>(fiszkiJson);
            var answers = JsonSerializer.Deserialize<Dictionary<int, string>>(answersJson) ?? new Dictionary<int, string>();

            if (fiszkiList == null || currentIndex >= fiszkiList.Count)
            {
                return RedirectToAction("TestResult");
            }

            var viewModel = new TestQuestionViewModel
            {
                TestId = fiszkiList[currentIndex].Id,
                Fiszka = fiszkiList[currentIndex],
                Direction = direction,
                CurrentIndex = currentIndex + 1,
                TotalCount = fiszkiList.Count,
                UserAnswer = answers.ContainsKey(currentIndex) ? answers[currentIndex] : ""
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TestQuestion(TestQuestionViewModel model)
        {
            var fiszkiJson = HttpContext.Session.GetString("TestFiszki");
            var direction = HttpContext.Session.GetString("TestDirection");
            var currentIndex = HttpContext.Session.GetInt32("TestCurrentIndex") ?? 0;
            var answersJson = HttpContext.Session.GetString("TestAnswers");

            if (string.IsNullOrEmpty(fiszkiJson) || string.IsNullOrEmpty(direction))
            {
                return RedirectToAction("Index");
            }

            var fiszkiList = JsonSerializer.Deserialize<List<Fiszki>>(fiszkiJson);
            var answers = JsonSerializer.Deserialize<Dictionary<int, string>>(answersJson) ?? new Dictionary<int, string>();

            if (fiszkiList == null || currentIndex >= fiszkiList.Count)
            {
                return RedirectToAction("TestResult");
            }

            answers[currentIndex] = model.UserAnswer;
            HttpContext.Session.SetString("TestAnswers", JsonSerializer.Serialize(answers));

            HttpContext.Session.SetInt32("TestCurrentIndex", currentIndex + 1);
            return RedirectToAction("TestQuestion");
        }

        public IActionResult TestResult()
        {
            var fiszkiJson = HttpContext.Session.GetString("TestFiszki");
            var direction = HttpContext.Session.GetString("TestDirection");
            var answersJson = HttpContext.Session.GetString("TestAnswers");
            var testId = HttpContext.Session.GetInt32("TestId") ?? 0;

            if (string.IsNullOrEmpty(fiszkiJson) || string.IsNullOrEmpty(direction) || string.IsNullOrEmpty(answersJson) || testId == 0)
            {
                return RedirectToAction("Index");
            }

            var fiszkiList = JsonSerializer.Deserialize<List<Fiszki>>(fiszkiJson);
            var answers = JsonSerializer.Deserialize<Dictionary<int, string>>(answersJson) ?? new Dictionary<int, string>();

            var test = _repository.Get(testId); // Use the correct TestId
            if (test == null)
            {
                Debug.WriteLine($"TestResult: Test with id={testId} not found.");
                return RedirectToAction("Index");
            }

            var result = new TestResultViewModel
            {
                TestId = test.Id,
                TestName = test.NazwaTestu,
                TotalCount = fiszkiList.Count,
                Answers = fiszkiList.Select((f, i) => (
                    f,
                    answers.ContainsKey(i) ? answers[i].Trim() : "",
                    direction == "EngToPol" ? f.Nazwa_PL.Trim().ToLower() == answers[i]?.Trim().ToLower() :
                                             f.Nazwa_ENGLISH.Trim().ToLower() == answers[i]?.Trim().ToLower()
                )).ToList(),
                Direction = direction // Add Direction to the view model
            };

            result.CorrectCount = result.Answers.Count(a => a.IsCorrect);

            // Save result to Wynik
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
            var wynik = new Wynik
            {
                Id_Testu = test.Id,
                Id_Uzytkownika = currentUserId,
                Id_Kategorii = test.Id_Kategorii,
                wynik = result.CorrectCount + "/" + result.TotalCount
            };
            _context.Wynik.Add(wynik);
            _context.SaveChanges();

            // Clear session
            HttpContext.Session.Remove("TestFiszki");
            HttpContext.Session.Remove("TestDirection");
            HttpContext.Session.Remove("TestCurrentIndex");
            HttpContext.Session.Remove("TestAnswers");
            HttpContext.Session.Remove("TestId");

            return View(result);
        }
    }
}