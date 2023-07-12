using Domain.Helpers;
using Domain.Interface;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;

        public EmailService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<string> SenEmail(string EmailAddress, string Username, string url)
        {
            try
            {
                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_appSettings.Mailer.Sender);
                mailMessage.To.Add(new MailAddress(EmailAddress));
                mailMessage.Subject = "Register confirmation";
                mailMessage.Body = $"<h1>Thank you dear {Username}!</h1>" +
                              "<p>You are 1 step close to finish registration process</P>" +
                              "<p>To finish up registration please follow link bellow</p>" +
                              $"<a href='{url}'>Click Here</a>";
                mailMessage.IsBodyHtml = true;

                var client = new SmtpClient(_appSettings.Mailer.Host)
                {
                    Port = _appSettings.Mailer.Port,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_appSettings.Mailer.Sender, _appSettings.Mailer.Password)
                };

                await client.SendMailAsync(mailMessage);
                return "success";
            }
            catch (SmtpException)
            {

                return null;
            }
        }
    }
}
