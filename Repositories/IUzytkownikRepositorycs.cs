namespace ProjektCRUD20510.Repositories
{
    public interface IUzytkownikRepositorycs
    {
        Models.Class Get(int IdUzytkownika);
        IQueryable<Models.Class> GetAll();
        void Add(Models.Class task);
        void Update(int IdUzytkownika, Models.Class uzytkownik);
        void Delete(int IdUzytkownika);
    }
}
