using Appointment.Api;
using Appointment.Api.Services.Mail;
using Appointment.Domain;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Appointment.Tests.Integration_
{
    public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        public readonly IServiceScope _scope;
        public readonly HttpClient _httpClient;
        //protected readonly IMailSendingService _mailSendingService;
        public readonly IUnitOfWork _uow;
        public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();
            _httpClient = factory.CreateClient();
            _uow = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        //    _mailSendingService = _scope.ServiceProvider.GetRequiredService<IMailSendingService>();
        }
    }
}
