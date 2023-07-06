using Domain.DTO.Authentication;
using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IRegisterService
    {
        void ActivateUser(User user);
        User GetUserByEmail(string email);
        User GetUserByUsername(string username);
        void RegisterUser(RegisterDTO registerDto);
        Task SendRegisterConfirmationEmail(string emailAddress, string username, string url);
    }
}
