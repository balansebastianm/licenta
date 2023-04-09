using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace LicWeb.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;
        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Course course)
        {
            _context.Add(course);
            return Save();
        }

        public bool Delete(Course course)
        {
            _context.Remove(course);
            return Save();
        }

        public async Task<IEnumerable<Course>> GetAll()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses.FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Course course)
        {
            _context.Update(course);
            return Save();
        }
        public async Task<Course> GetIdByCourseName(string name)
        {
            return await _context.Courses.FirstOrDefaultAsync(i => i.CourseName == name);
        }
    }
}
