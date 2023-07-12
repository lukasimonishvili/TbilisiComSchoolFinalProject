using Domain.DTO.Authentication;

namespace Domain.Interface
{
    public interface ILoginService
    {
        string Authenticate(LoginDTO login);
        string RefreshToken(string tokenString);
        UserDTO GetUserById(int userId, string authorizationHeader, bool IsTest = false);
    }
}
