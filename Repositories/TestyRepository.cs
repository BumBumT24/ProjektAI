using Microsoft.EntityFrameworkCore;
using ProjektCRUD20510.Models;

namespace ProjektCRUD20510.Repositories
{
    public class TestyRepository : ITestyRepository
    {
        private readonly ClassManagerContext _context;

        public TestyRepository(ClassManagerContext context)
        {
            _context = context;
        }

        public Testy Get(int id) => _context.Testy
            .Include(t => t.Kategoria)
            .Include(t => t.Testy_Fiszki)
                .ThenInclude(tf => tf.Fiszka)
            .SingleOrDefault(x => x.Id == id);

        public IQueryable<Testy> GetAll() => _context.Testy
            .Include(t => t.Kategoria);

        public void Add(Testy testy)
        {
            _context.Testy.Add(testy);
            _context.SaveChanges();
        }

        public void Update(int id, Testy testy)
        {
            var result = _context.Testy
                .Include(t => t.Testy_Fiszki)
                .SingleOrDefault(x => x.Id == id);
            if (result != null)
            {
                result.Id_Kategorii = testy.Id_Kategorii;
                result.NazwaTestu = testy.NazwaTestu;

                // Update Testy_Fiszki relationships
                _context.Testy_Fiszki.RemoveRange(result.Testy_Fiszki); // Remove existing relationships
                result.Testy_Fiszki = testy.Testy_Fiszki; // Add new relationships
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var result = _context.Testy.SingleOrDefault(x => x.Id == id);
            if (result != null)
            {
                _context.Testy.Remove(result);
                _context.SaveChanges();
            }
        }
    }
}