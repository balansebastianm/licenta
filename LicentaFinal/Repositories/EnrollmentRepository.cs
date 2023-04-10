using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace LicWeb.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Enrollment enrollment)
        {
            _context.Add(enrollment);
            return Save();
        }

        public bool Delete(Enrollment enrollment)
        {
            _context.Remove(enrollment);
            return Save();
        }

        public async Task<IEnumerable<Enrollment>> GetAll()
        {
            return await _context.Enrollments.ToListAsync();
        }

        public async Task<Enrollment> GetByIdAsync(int id)
        {
            return await _context.Enrollments.FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Enrollment enrollment)
        {
            _context.Update(enrollment);
            return Save();
        }
    }
}
