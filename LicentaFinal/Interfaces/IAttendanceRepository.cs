using LicWeb.Models;

namespace LicWeb.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAll();
        Task<Attendance> GetByIdAsync(int id);
        bool Add(Attendance attendance);
        bool Update(Attendance attendance);
        bool Delete(Attendance attendance);
        bool Save();
        Task<Attendance> GetIdByEnrollmentId (int enrollmentId);
    }
}
