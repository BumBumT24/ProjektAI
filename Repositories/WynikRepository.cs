using Microsoft.EntityFrameworkCore;
using ProjektCRUD20510.Models;

namespace ProjektCRUD20510.Repositories
{
    public class WynikRepository(ClassManagerContext context) : IWynikRepository
    {
        private readonly ClassManagerContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public Wynik? Get(int id) => _context.Wynik
            .Include(w => w.Kategoria)
            .Include(w => w.Test)
            .Include(w => w.Uzytkownik)
            .SingleOrDefault(x => x.Id == id);

        public IQueryable<Wynik> GetAll() => _context.Wynik
            .Include(w => w.Kategoria)
            .Include(w => w.Test)
            .Include(w => w.Uzytkownik);

        public void Add(Wynik wynik)
        {
            ArgumentNullException.ThrowIfNull(wynik);
            _context.Wynik.Add(wynik);
            _context.SaveChanges();
        }

        public void Update(int id, Wynik wynik)
        {
            var result = _context.Wynik.SingleOrDefault(x => x.Id == id);
            if (result != null)
            {
                result.Id_Testu = wynik.Id_Testu;
                result.Id_Uzytkownika = wynik.Id_Uzytkownika;
                result.Id_Kategorii = wynik.Id_Kategorii;
                result.wynik = wynik.wynik;
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var result = _context.Wynik.SingleOrDefault(x => x.Id == id);
            if (result != null)
            {
                _context.Wynik.Remove(result);
                _context.SaveChanges();
            }
        }
    }
}