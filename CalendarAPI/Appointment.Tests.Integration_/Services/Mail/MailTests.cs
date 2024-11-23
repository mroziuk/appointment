using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Tests.Integration_.Services.Mail
{
    public class MailTests : BaseIntegrationTest
    {
        public MailTests(IntegrationTestWebAppFactory factory)
            : base(factory)
        {
        }
        //[Fact]
        public async Task MailService_SendMail_Correctly()
        {
            string to = "mikolaj.mroziuk@gmail.com";
            string subject = "Test";
            string body = "Test";
            //await _mailSendingService.SendMailAsync(to, subject, body);

        }
    }
}
