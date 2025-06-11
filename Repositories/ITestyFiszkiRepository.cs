using ProjektCRUD20510.Models;

namespace ProjektCRUD20510.Repositories
{
    public interface ITestyFiszkiRepository
    {
        Testy_Fiszki? Get(int idTestu, int idFiszki);
        IQueryable<Testy_Fiszki> GetAll();
        void Add(Testy_Fiszki testyFiszki);
        void Update(int idTestu, int idFiszki, Testy_Fiszki testyFiszki);
        void Delete(int idTestu, int idFiszki);
    }
}