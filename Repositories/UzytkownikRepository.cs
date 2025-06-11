namespace ProjektCRUD20510.Repositories
{
    public class UzytkownikRepository : IUzytkownikRepositorycs
    {
        private readonly Models.ClassManagerContext _context;
        public UzytkownikRepository(Models.ClassManagerContext context)
        {
            _context = context;
        }
        public Models.Class Get(int IdUzytkownika) => _context.Uzytkownik.SingleOrDefault(x => x.Id == IdUzytkownika);
        public IQueryable<Models.Class> GetAll() => _context.Uzytkownik;
        
        public void Add(Models.Class klasa)
        {
            _context.Uzytkownik.Add(klasa);
            _context.SaveChanges();
        }
        public void Update(int IdUzytkownika,  Models.Class klasa)
        {
            var result = _context.Uzytkownik.SingleOrDefault(x=>x.Id == IdUzytkownika);
            if (result != null)
            {
                result.Nazwa = klasa.Nazwa;
                result.Haslo = klasa.Haslo;
                result.Role = klasa.Role;
                _context.SaveChanges();
            }
        }
        public void Delete(int IdUzytkownika)
        {
            var result = _context.Uzytkownik.SingleOrDefault(x => x.Id == IdUzytkownika);
            if(result != null)
            {
                _context.Uzytkownik.Remove(result);
                _context.SaveChanges();
            }
        }
        
    }
}
