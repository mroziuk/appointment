using Microsoft.AspNetCore.Authentication;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Appointment.Tests.Integration_.Handlers
{
    public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOption>
    {
        public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationOption> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authenticaionTicket = new AuthenticationTicket(
                new ClaimsPrincipal(Options.Identity),
                new AuthenticationProperties(),
                "Bearer"
                );
            return Task.FromResult(AuthenticateResult.Success(authenticaionTicket));
        }
    }

    public static class TestAuthenticationExtensions
    {
        public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<TestAuthenticationOption> configureOptions)
        {
            return builder.AddScheme<TestAuthenticationOption, TestAuthenticationHandler>("Bearer", "Bearer", configureOptions);
        }
    }

    public class TestAuthenticationOption : AuthenticationSchemeOptions
    {
        public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(new Claim[]
        {
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "1"),
        }, "test");
    }
}
