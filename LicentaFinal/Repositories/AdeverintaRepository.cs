using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using Microsoft.EntityFrameworkCore;
namespace LicWeb.Repositories
{
    public class AdeverintaRepository : IAdeverintaRepository
    {
        private readonly ApplicationDbContext _context;
        public AdeverintaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Adeverinta adeverinta)
        {
            _context.Add(adeverinta);
            return Save();
        }

        public bool Delete(Adeverinta adeverinta)
        {
            _context.Remove(adeverinta);
            return Save();
        }

        public async Task<IEnumerable<Adeverinta>> GetAll()
        {
            return await _context.Adeverinte.ToListAsync();
        }

        public async Task<Adeverinta> GetByIdAsync(int id)
        {
            return await _context.Adeverinte.FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Adeverinta adeverinta)
        {
            _context.Update(adeverinta);
            return Save();
        }
    }
}
