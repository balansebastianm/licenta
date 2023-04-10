using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace LicWeb.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;
        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Attendance attendance)
        {
            _context.Add(attendance);
            return Save();
        }

        public bool Delete(Attendance attendance)
        {
            _context.Remove(attendance);
            return Save();
        }

        public async Task<IEnumerable<Attendance>> GetAll()
        {
            return await _context.Attendances.ToListAsync();
        }

        public async Task<Attendance> GetByIdAsync(int id)
        {
            return await _context.Attendances.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Attendance> GetIdByEnrollmentId(int enrollmentId)
        {
            return await _context.Attendances.FirstOrDefaultAsync(i => i.EnrollmentId == enrollmentId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Attendance attendance)
        {
            _context.Update(attendance);
            return Save();
        }
    }
}
