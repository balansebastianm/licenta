using LicWeb.Models;

namespace LicWeb.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAll();
        Task<Doctor> GetByIdAsync(int id);
        bool Add(Doctor doctor);
        bool Update(Doctor doctor);
        bool Delete(Doctor doctor);
        bool Save();
        Doctor GetByUID(string Id);
    }
}
