using BCryptNet = BCrypt.Net.BCrypt;
using System.Net;
using System.Net.Mail;
using Domain.DTO.Authentication;
using Domain.Entities;
using Infrastructure.Repositories;
using Mapster;
using System.Threading.Tasks;
using Domain.Helpers;
using Domain.Interface;

namespace Infrastructure.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly MailerSettings _mailerSettings;
        private readonly UserRepository _userRepository;
        public RegisterService(AppSettings appSettings, UserRepository userRepository)
        {
            _mailerSettings = appSettings.Mailer;
            _userRepository = userRepository;
        }


        public void RegisterUser(RegisterDTO registerDto)
        {
            var AdapdetUser = registerDto.Adapt<User>();
            AdapdetUser.Password = BCryptNet.HashPassword(AdapdetUser.Password);
            AdapdetUser.IsBlocked = false;
            AdapdetUser.Verified = false;
            AdapdetUser.Role = Roles.User;
            _userRepository.AddUserToDataBase(AdapdetUser);
        }

        public Task SendRegisterConfirmationEmail(string emailAddress, string username, string url)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_mailerSettings.Sender);
            mailMessage.To.Add(new MailAddress(emailAddress));
            mailMessage.Subject = "Register confirmation";
            mailMessage.Body = $"<h1>Thank you dear {username}!</h1>" +
                          "<p>You are 1 step close to finish registration process</P>" +
                          "<p>To finish up registration please follow link bellow</p>" +
                          $"<a href='{url}'>Click Here</a>";
            mailMessage.IsBodyHtml = true;

            var client = new SmtpClient(_mailerSettings.Host)
            {
                Port = _mailerSettings.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_mailerSettings.Sender, _mailerSettings.Password)
            };

            return client.SendMailAsync(mailMessage);
        }


        public User GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }

        public User GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }

        public void ActivateUser(User user)
        {
            var updatedUser = user;
            updatedUser.Verified = true;
            _userRepository.UpdateUser(updatedUser);
        }
    }
}
