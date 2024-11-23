namespace Appointment.Api.Services.Mail
{
    public interface IMailSendingService
    {
        Task SendMailAsync(string to, string subject, string body);
        Task SendConfirmationMail(string to, string token);
        Task SendResetPasswordMail(string to, string url);
    }
}
