using Domain.Entities;
using Domain.Interface;
using Infrastructure.Persistence;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly static DataBaseContext _context = new();

        public User AddUserToDataBase(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
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

        public string UpdateUser(User newUser)
        {
            var oldUser = _context.Users.FirstOrDefault(u => u.Id == newUser.Id);
            _context.Entry(oldUser).CurrentValues.SetValues(newUser);
            _context.SaveChanges();

            return "success";
        }

    }
}

