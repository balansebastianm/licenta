using LicWeb.Models;

namespace LicWeb.Interfaces
{
    public interface ISeminarRepository
    {
        Task<IEnumerable<Seminar>> GetAll();
        Task<Seminar> GetByIdAsync(int id);
        bool Add(Seminar seminar);
        bool Update(Seminar seminar);
        bool Delete(Seminar seminar);
        bool Save();
        int GetProfByAssocCourse(int courseId);
        Task<Seminar> GetByCursId(int id);
    }
}
