using Domain.Entities;

namespace Domain.Interface
{
    public interface IUserRepository
    {
        User AddUserToDataBase(User user);
        User GetUserByEmail(string email);
        User GetUserById(int id);
        User GetUserByUsername(string username);
        string UpdateUser(User newUser);
    }
}