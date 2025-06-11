namespace ProjektCRUD20510.Repositories
{
    public interface IFiszkiRepository
    {
        Models.Fiszki Get(int id);
        IQueryable<Models.Fiszki> GetAll();
        void Add(Models.Fiszki fiszki);
        void Update(int id, Models.Fiszki fiszki);
        void Delete(int id);
    }
}
