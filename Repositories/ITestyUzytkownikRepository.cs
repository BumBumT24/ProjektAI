namespace ProjektCRUD20510.Repositories
{
    public interface ITestyUzytkownikRepository
    {
        Models.Testy_Uzytkownik Get(int idTestu, int idUzytkownika);
        IQueryable<Models.Testy_Uzytkownik> GetAll();
        void Add(Models.Testy_Uzytkownik testyUzytkownik);
        void Update(int idTestu, int idUzytkownika, Models.Testy_Uzytkownik testyUzytkownik);
        void Delete(int idTestu, int idUzytkownika);
    }
}