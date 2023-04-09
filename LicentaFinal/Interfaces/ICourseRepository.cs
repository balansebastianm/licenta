using LicWeb.Models;

namespace LicWeb.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAll();
        Task<Course> GetByIdAsync(int id);
        bool Add(Course course);
        bool Update(Course course);
        bool Delete(Course course);
        bool Save();
        Task<Course> GetIdByCourseName(string name);
    }
}
