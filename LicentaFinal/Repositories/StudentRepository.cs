using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace LicWeb.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Student student)
        {
            _context.Add(student);
            return Save();
        }

        public bool Delete(Student student)
        {
            _context.Remove(student);
            return Save();
        }

        public async Task<IEnumerable<Student>> GetAll()
        {
            return await _context.Studenti.ToListAsync();
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Studenti.FirstOrDefaultAsync(i => i.Id == id);
        }

        public int GetIdByUID(string uid)
        {
            var query = _context.Studenti.Where(b => b.StudentUserId == uid).ToList();
            int StudentId = query.Select(x => x.Id).ToList().FirstOrDefault();
            return StudentId;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Student student)
        {
            _context.Update(student);
            return Save();
        }
    }
}
