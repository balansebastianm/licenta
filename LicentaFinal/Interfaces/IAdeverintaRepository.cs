using LicWeb.Models;

namespace LicWeb.Interfaces
{
    public interface IAdeverintaRepository
    {
        Task<IEnumerable<Adeverinta>> GetAll();
        Task<Adeverinta> GetByIdAsync(int id);
        bool Add(Adeverinta adeverinta);
        bool Update(Adeverinta adeverinta);
        bool Delete(Adeverinta adeverinta);
        bool Save();
    }
}
