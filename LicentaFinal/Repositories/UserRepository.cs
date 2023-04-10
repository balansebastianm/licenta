using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using LicWeb.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LicWeb.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(User user)
        {
            _context.Add(user);
            return Save();
        }

        public bool Delete(User user)
        {
            _context.Remove(user);
            return Save();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(User user)
        {
            _context.Update(user);
            return Save();
        }
        public string GetIdByToken(string token)
        {
            var query = _context.Users.Where(b => b.TokenInregistrare == token).ToList();
            string UserId = query.Select(x => x.Id).ToList().FirstOrDefault();
            return UserId;
        }

        public string GetIdByEmail(string email)
        {
            var query = _context.Users.Where(b => b.Email == email).ToList();
            string UserId = query.Select(x => x.Id).ToList().FirstOrDefault();
            return UserId;
        }
    }
}
