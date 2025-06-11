namespace ProjektCRUD20510.Repositories
{
    public interface IWynikRepository
    {
        Models.Wynik? Get(int id);
        IQueryable<Models.Wynik> GetAll();
        void Add(Models.Wynik wynik);
        void Update(int id, Models.Wynik wynik);
        void Delete(int id);
    }
}