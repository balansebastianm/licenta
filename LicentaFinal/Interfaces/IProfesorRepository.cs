using LicWeb.Models;

namespace LicWeb.Interfaces
{
    public interface IProfesorRepository
    {
        Task<IEnumerable<Profesor>> GetAll();
        Task<Profesor> GetByIdAsync(int id);
        bool Add(Profesor profesor);
        bool Update(Profesor profesor);
        bool Delete(Profesor profesor);
        bool Save();
        string GetUserIdFromProfId(int id);
    }
}
