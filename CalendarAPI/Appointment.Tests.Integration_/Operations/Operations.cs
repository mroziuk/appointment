using Appointment.Api.Services.Auth.Interfaces;
using Appointment.Api.Services.Auth;
using Appointment.Data;
using Appointment.Domain.Services.Auth;
using Appointment.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Appointment.Api.Services.Mail;
using Moq;
namespace Appointment.Tests.Integration_.Operations
{
    public class Operations
    {
        private readonly IConfiguration _configuration;
        protected readonly CalendarContext _context;
        protected readonly IIdentityService _identityService;
        protected readonly IJwtProvider _jwtProvider;
        protected readonly IRng _rng;
        protected readonly IPasswordService _passwordService;
        protected readonly IRefreshTokenService _refreshTokenService;
        protected readonly IUnitOfWork _uow;
        protected readonly IMailSendingService _mailSendingService;

        protected Operations()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"JwtIssuerOptions:IssuerSigningKey", "test1test2test3test4test5test6test7"},
                {"JwtIssuerOptions:Issuer", "test"},
                {"JwtIssuerOptions:Algorithm", "HS256"},
                {"JwtIssuerOptions:ExpiryMinutes", "60"},
                {"JwtIssuerOptions:Audience", "https://localhost:5005/"},
                {"RefreshTokenOptions:ExpiryMinutes", "525600"},
                {"GuestSecretKey", "test"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _context = new InMemoryContext();
            _uow = new UnitOfWork(_context);
            _jwtProvider = new JwtProvider(_configuration);
            _passwordService = new PasswordService(new PasswordHasher<IPasswordService>());
            _rng = new Rng();
            _refreshTokenService = new RefreshTokenService(_jwtProvider, _uow, _rng, _configuration);

            Mock<IMailSendingService> mailSendingService = new Mock<IMailSendingService>();
            mailSendingService.Setup(x => x.SendResetPasswordMail(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            mailSendingService.Setup(x => x.SendConfirmationMail(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            mailSendingService.Setup(x => x.SendMailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _mailSendingService = mailSendingService.Object;

            _identityService = new IdentityService(_refreshTokenService, _passwordService, _jwtProvider, _uow, _rng, _mailSendingService);
            
        }

    }
}
