using Microsoft.EntityFrameworkCore;
using ProjektCRUD20510.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjektCRUD20510.Repositories
{
    public class TestyUzytkownikRepository : ITestyUzytkownikRepository
    {
        private readonly ClassManagerContext _context;

        public TestyUzytkownikRepository(ClassManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Testy_Uzytkownik Get(int idTestu, int idUzytkownika)
        {
            return _context.Testy_Uzytkownik
                .Include(tu => tu.Test)
                .Include(tu => tu.Uzytkownik)
                .FirstOrDefault(tu => tu.Id_Testu == idTestu && tu.Id_Uzytkownika == idUzytkownika);
        }

        public IQueryable<Testy_Uzytkownik> GetAll()
        {
            return _context.Testy_Uzytkownik
                .Include(tu => tu.Test)
                .Include(tu => tu.Uzytkownik)
                .AsQueryable();
        }

        public void Add(Testy_Uzytkownik testyUzytkownik)
        {
            if (testyUzytkownik == null)
            {
                throw new ArgumentNullException(nameof(testyUzytkownik));
            }

            _context.Testy_Uzytkownik.Add(testyUzytkownik);
            _context.SaveChanges();
        }

        public void Update(int idTestu, int idUzytkownika, Testy_Uzytkownik testyUzytkownik)
        {
            var existing = Get(idTestu, idUzytkownika);
            if (existing != null)
            {
                // Since this is a composite key entity, keys (Id_Testu, Id_Uzytkownika) shouldn't change
                // Update other properties if they exist (none in this model, so no action needed)
                _context.SaveChanges();
            }
        }

        public void Delete(int idTestu, int idUzytkownika)
        {
            var existing = Get(idTestu, idUzytkownika);
            if (existing != null)
            {
                _context.Testy_Uzytkownik.Remove(existing);
                _context.SaveChanges();
            }
        }
    }
}