using Domain.DTO.Authentication;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IRegisterService
    {
        string ActivateUser(string Email);
        Task<string> RegisterUser(RegisterDTO registerDto, string url);
    }
}
