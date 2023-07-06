using Domain.Entities;
using Infrastructure.Persistence;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly static DataBaseContext _context = new();

        public void AddUserToDataBase(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public void UpdateUser(User newUser)
        {
            var oldUser = _context.Users.FirstOrDefault(u => u.Id == newUser.Id);
            _context.Entry(oldUser).CurrentValues.SetValues(newUser);
            _context.SaveChanges();
        }

    }
}

