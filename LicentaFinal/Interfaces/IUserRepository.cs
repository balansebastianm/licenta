using LicWeb.Models;

namespace LicWeb.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetByIdAsync(string id);
        bool Add(User user);
        bool Update(User user);
        bool Delete(User user);
        bool Save();
        string GetIdByToken(string token);
        string GetIdByEmail(string email);

    }
}
