using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace LicWeb.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;


        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(DoctorFamilie doctorFamilie)
        {
            _context.Add(doctorFamilie);
            return Save();
        }

        public bool Delete(DoctorFamilie doctorFamilie)
        {
            _context.Remove(doctorFamilie);
            return Save();
        }

        public async Task<IEnumerable<DoctorFamilie>> GetAll()
        {
            return await _context.DoctoriFamilie.ToListAsync();
        }

        public async Task<DoctorFamilie> GetByIdAsync(int id)
        {
            return await _context.DoctoriFamilie.FirstOrDefaultAsync(i => i.Id == id);
        }

        public DoctorFamilie GetByUID(string Id)
        {
            return _context.DoctoriFamilie.FirstOrDefault(b => b.DoctorUserId == Id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(DoctorFamilie doctorFamilie)
        {
            _context.Update(doctorFamilie);
            return Save();
        }
        
    }
}
