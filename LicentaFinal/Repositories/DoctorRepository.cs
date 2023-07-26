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

        public bool Add(Doctor doctorFamilie)
        {
            _context.Add(doctorFamilie);
            return Save();
        }

        public bool Delete(Doctor doctorFamilie)
        {
            _context.Remove(doctorFamilie);
            return Save();
        }

        public async Task<IEnumerable<Doctor>> GetAll()
        {
            return await _context.Doctori.ToListAsync();
        }

        public async Task<Doctor> GetByIdAsync(int id)
        {
            return await _context.Doctori.FirstOrDefaultAsync(i => i.Id == id);
        }

        public Doctor GetByUID(string Id)
        {
            return _context.Doctori.FirstOrDefault(b => b.DoctorUserId == Id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Doctor doctorFamilie)
        {
            _context.Update(doctorFamilie);
            return Save();
        }
        
    }
}
