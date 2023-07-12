using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IEmailService
    {
        Task<string> SenEmail(string EmailAddress, string Username, string url);
    }
}
