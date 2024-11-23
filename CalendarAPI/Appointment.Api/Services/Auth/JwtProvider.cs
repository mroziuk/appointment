using Appointment.Domain.DTO.Identity;
using Appointment.Domain.Entities;
using Appointment.Api.Services.Auth.Interfaces;
using System.Security.Claims;
using Appointment.Common.Models.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Appointment.Api.Services.Auth
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _jwtConfiguration;

        public JwtProvider(IConfiguration jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration.GetSection(nameof(JwtIssuerOptions));
        }

        public AuthTokenDto Create(User user, List<Claim> claims = null)
        {
            return Create(user.Id.ToString(), user.Role, claims);
        }

        public AuthTokenDto Create(string id, string role, List<Claim> claims = null)
        {
            var utcNow = DateTime.UtcNow;
            var expires = utcNow.AddMinutes(Convert.ToInt32(_jwtConfiguration[nameof(JwtIssuerOptions.ExpiryMinutes)]));
            var claimsList = new List<Claim>
            {
                new Claim("jti", Guid.NewGuid().ToString()), // jwt id
                new Claim("sub", id), // subject
                new Claim("role", role), // role
                new Claim("iat", ConvertToTimestamp(utcNow).ToString()), // issued at
                new Claim("nbf", ConvertToTimestamp(utcNow).ToString()), // not before
                new Claim("exp", ConvertToTimestamp(expires).ToString()), // expires
            };

            claims?.ForEach(claim => claimsList.Add(claim));

            var token = new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(
                    _jwtConfiguration[nameof(JwtIssuerOptions.Issuer)],
                    claims: claimsList,
                    notBefore: utcNow,
                    expires: expires,
                    signingCredentials: new SigningCredentials
                        (new SymmetricSecurityKey
                            (Encoding.UTF8.GetBytes(_jwtConfiguration[nameof(JwtIssuerOptions.IssuerSigningKey)])),
                            _jwtConfiguration[nameof(JwtIssuerOptions.Algorithm)])));
            return new AuthTokenDto
            {
                AccessToken = token,
                Expires = ConvertToTimestamp(expires),
                Role = role
            };
        }
        public static long ConvertToTimestamp(DateTime value)
        {
            var elapsedTime = value - DateTime.UnixEpoch;
            return (long)elapsedTime.TotalSeconds;
        }
    }
}
