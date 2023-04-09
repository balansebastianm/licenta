using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace LicWeb.Repositories
{
    public class SeminarRepository : ISeminarRepository
    {
        private readonly ApplicationDbContext _context;
        public SeminarRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Seminar seminar)
        {
            _context.Add(seminar);
            return Save();
        }

        public bool Delete(Seminar seminar)
        {
            _context.Remove(seminar);
            return Save();
        }

        public async Task<IEnumerable<Seminar>> GetAll()
        {
            return await _context.Seminars.ToListAsync();
        }

        public async Task<Seminar> GetByIdAsync(int id)
        {
            return await _context.Seminars.SingleOrDefaultAsync(x => x.Id.Equals(id));
        }
        public async Task<Seminar> GetByCursId(int id)
        {
            return await _context.Seminars.SingleOrDefaultAsync(x => x.IdCursAsociat.Equals(id));
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Seminar seminar)
        {
            _context.Update(seminar);
            return Save();
        }
        public int GetProfByAssocCourse(int courseId)
        {
            var query = _context.Seminars.Where(b => b.IdCursAsociat == courseId).ToList();
            int ProfId = query.Select(x => x.ProfesorSeminarId).ToList().FirstOrDefault();
            return ProfId;
        }
    }
}
