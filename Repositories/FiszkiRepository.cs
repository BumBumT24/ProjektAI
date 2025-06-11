using Microsoft.EntityFrameworkCore;

namespace ProjektCRUD20510.Repositories
{
    public class FiszkiRepository : IFiszkiRepository
    {
        private readonly Models.ClassManagerContext _context;

        public FiszkiRepository(Models.ClassManagerContext context)
        {
            _context = context;
        }

        public Models.Fiszki Get(int id) => _context.Fiszki
            .Include(f => f.Uzytkownik)
            .SingleOrDefault(x => x.Id == id);

        public IQueryable<Models.Fiszki> GetAll() => _context.Fiszki.Include(f => f.Uzytkownik);

        public void Add(Models.Fiszki fiszki)
        {
            _context.Fiszki.Add(fiszki);
            _context.SaveChanges();
        }

        public void Update(int id, Models.Fiszki fiszki)
        {
            var existing = _context.Fiszki.Find(id);
            if (existing != null)
            {
                existing.Id_Uzytkownika = fiszki.Id_Uzytkownika;
                existing.Nazwa_PL = fiszki.Nazwa_PL;
                existing.Nazwa_ENGLISH = fiszki.Nazwa_ENGLISH;
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var existing = _context.Fiszki.Find(id);
            if (existing != null)
            {
                _context.Fiszki.Remove(existing);
                _context.SaveChanges();
            }
        }
    }
}
