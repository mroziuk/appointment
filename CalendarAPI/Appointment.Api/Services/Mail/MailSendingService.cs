using Appointment.Domain;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using static Appointment.Common.Utils.HttpHelper;
namespace Appointment.Api.Services.Mail
{
    public class MailSendingService : IMailSendingService
    {
        private readonly SmtpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MailSendingService(IHttpContextAccessor httpContextAccessor)
        {
            _client = new SmtpClient();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendConfirmationMail(string to, string token)
        {
            var uri = GetAbsoluteUri();
            await SendMailAsync(to, "Confirm your email", $"Please confirm your email by clicking <a href='{uri}{token}'>here</a>");
        }

        public async Task SendMailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("app.ointment", "app.appointment.net@gmail.com"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            await SendMessage(message);
        }

        public async Task SendResetPasswordMail(string to, string token)
        {
            var uri = GetAbsoluteUri();
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("app.ointment", "app.appointment.net@gmail.com"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = "Reset your password";
            var text = 
@$"<form method=""post"" action=""{uri}{Urls.ActivateAccount}"" class=""inline"">
<input type=""hidden"" name=""token"" value=""{token}"">
<button type=""submit"" name=""submit_param"" value=""submit_value"" class=""link-button"">
    This is a link that sends a POST request
</button>
</form>";
            message.Body = new TextPart("html") { Text = text };

            await SendMessage(message);
        }
        private async Task SendMessage(MimeMessage message)
        {
            await _client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await _client.AuthenticateAsync("app.appointment.net@gmail.com", "fimhnjhgayinpkix");
            await _client.SendAsync(message);
            await _client.DisconnectAsync(true);
        }
        private Uri GetAbsoluteUri()
        {
            #if DEBUG
                return new Uri("https://localhost:44344/");
            #endif

            if(_httpContextAccessor.HttpContext != null)
            {
                var request = _httpContextAccessor.HttpContext.Request;
                UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = request.Scheme;
                uriBuilder.Host = request.Host.Host;
                return uriBuilder.Uri;
            }

            return null;
        }
    }
}
