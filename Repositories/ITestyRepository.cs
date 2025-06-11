namespace ProjektCRUD20510.Repositories
{
    public interface ITestyRepository
    {
        Models.Testy Get(int id);
        IQueryable<Models.Testy> GetAll();
        void Add(Models.Testy testy);
        void Update(int id, Models.Testy testy);
        void Delete(int id);
    }
}