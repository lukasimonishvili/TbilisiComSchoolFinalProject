namespace Domain.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public MailerSettings Mailer { get; set; }
    }
}
