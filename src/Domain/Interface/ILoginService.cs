using Domain.DTO.Authentication;

namespace Domain.Interface
{
    public interface ILoginService
    {
        string Authenticate(LoginDTO login);
        string RefreshToken(string tokenString);
    }
}
