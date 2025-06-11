using Microsoft.EntityFrameworkCore;
using ProjektCRUD20510.Models;

namespace ProjektCRUD20510.Repositories
{
    public class TestyFiszkiRepository(ClassManagerContext context) : ITestyFiszkiRepository
    {
        private readonly ClassManagerContext _context = context;

        public Testy_Fiszki? Get(int idTestu, int idFiszki) => _context.Testy_Fiszki
            .Include(t => t.Test)
            .Include(t => t.Fiszka)
            .SingleOrDefault(x => x.Id_Testu == idTestu && x.Id_Fiszki == idFiszki);

        public IQueryable<Testy_Fiszki> GetAll() => _context.Testy_Fiszki
            .Include(t => t.Test)
            .Include(t => t.Fiszka);

        public void Add(Testy_Fiszki testyFiszki)
        {
            _context.Testy_Fiszki.Add(testyFiszki);
            _context.SaveChanges();
        }

        public void Update(int idTestu, int idFiszki, Testy_Fiszki testyFiszki)
        {
            var result = _context.Testy_Fiszki
                .SingleOrDefault(x => x.Id_Testu == idTestu && x.Id_Fiszki == idFiszki);
            if (result != null)
            {
                result.Id_Testu = testyFiszki.Id_Testu;
                result.Id_Fiszki = testyFiszki.Id_Fiszki;
                _context.SaveChanges();
            }
        }

        public void Delete(int idTestu, int idFiszki)
        {
            var result = _context.Testy_Fiszki
                .SingleOrDefault(x => x.Id_Testu == idTestu && x.Id_Fiszki == idFiszki);
            if (result != null)
            {
                _context.Testy_Fiszki.Remove(result);
                _context.SaveChanges();
            }
        }
    }
}