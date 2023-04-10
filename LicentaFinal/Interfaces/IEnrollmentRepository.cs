using LicWeb.Models;

namespace LicWeb.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetAll();
        Task<Enrollment> GetByIdAsync(int id);
        bool Add(Enrollment enrollment);
        bool Update(Enrollment enrollment);
        bool Delete(Enrollment enrollment);
        bool Save();
    }
}
