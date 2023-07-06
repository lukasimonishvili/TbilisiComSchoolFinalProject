using Domain.Entities;
using Infrastructure.Persistence;
using System.Linq;

namespace Infrastructure.Repositories
{
    internal class UserRepository
    {
        private readonly static DataBaseContext _context = new();

        public static void AddUserToDataBase(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public static User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public static User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
    }
}

