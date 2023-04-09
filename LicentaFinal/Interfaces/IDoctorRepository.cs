using LicWeb.Models;

namespace LicWeb.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<DoctorFamilie>> GetAll();
        Task<DoctorFamilie> GetByIdAsync(int id);
        bool Add(DoctorFamilie doctorFamilie);
        bool Update(DoctorFamilie doctorFamilie);
        bool Delete(DoctorFamilie doctorFamilie);
        bool Save();
    }
}
