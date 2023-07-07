using Domain.Entities;

namespace Domain.Interface
{
    public interface IUserRepository
    {
        void AddUserToDataBase(User user);
        User GetUserByEmail(string email);
        User GetUserById(int id);
        User GetUserByUsername(string username);
        void UpdateUser(User newUser);
    }
}