namespace ProjektCRUD20510.Repositories
{
    public interface IKategorieRepository
    {
        Models.Kategorie Get(int id);
        IQueryable<Models.Kategorie> GetAll();
        void Add(Models.Kategorie kategorie);
        void Update(int id, Models.Kategorie kategorie);
        void Delete(int id);
    }
}