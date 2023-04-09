using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace LicWeb.Repositories
{
    public class ProfesorRepository : IProfesorRepository
    {
        private readonly ApplicationDbContext _context;
        public ProfesorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Profesor profesor)
        {
            _context.Add(profesor);
            return Save();
        }

        public bool Delete(Profesor profesor)
        {
            _context.Remove(profesor);
            return Save();
        }

        public async Task<IEnumerable<Profesor>> GetAll()
        {
            return await _context.Profesori.ToListAsync();
        }

        public async Task<Profesor> GetByIdAsync(int id)
        {

            return await _context.Profesori.SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Profesor profesor)
        {
            _context.Update(profesor);
            return Save();
        }
        public string GetUserIdFromProfId(int id)
        {
            var query = _context.Profesori.Where(b => b.Id == id).ToList();
            string ProfUserId = query.Select(x => x.ProfesorUserId).ToList().FirstOrDefault();
            return ProfUserId;
        }
    }
}
