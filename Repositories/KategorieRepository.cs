using Microsoft.EntityFrameworkCore;

namespace ProjektCRUD20510.Repositories
{
    public class KategorieRepository : IKategorieRepository
    {
        private readonly Models.ClassManagerContext _context;

        public KategorieRepository(Models.ClassManagerContext context)
        {
            _context = context;
        }

        public Models.Kategorie Get(int id) => _context.Kategorie.SingleOrDefault(x => x.Id == id);

        public IQueryable<Models.Kategorie> GetAll() => _context.Kategorie;

        public void Add(Models.Kategorie kategorie)
        {
            _context.Kategorie.Add(kategorie);
            _context.SaveChanges();
        }

        public void Update(int id, Models.Kategorie kategorie)
        {
            var result = _context.Kategorie.SingleOrDefault(x => x.Id == id);
            if (result != null)
            {
                result.Nazwa = kategorie.Nazwa;
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var result = _context.Kategorie.SingleOrDefault(x => x.Id == id);
            if (result != null)
            {
                _context.Kategorie.Remove(result);
                _context.SaveChanges();
            }
        }
    }
}