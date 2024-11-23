using Appointment.Api.Services.Auth.Interfaces;
using Appointment.Data;
using Appointment.Domain.DTO.Identity;
using Appointment.Domain.Entities;
using Appointment.Domain.Exceptions.Identity;
using Microsoft.EntityFrameworkCore;

namespace Appointment.Domain.Services.Auth
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _uow;
        private readonly IRng _rng;
        private readonly IConfiguration _refreshTokenConfiguration;

        public RefreshTokenService(IJwtProvider jwtProvider, IUnitOfWork uow, IRng rng, IConfiguration configuration)
        {
            _jwtProvider = jwtProvider;
            _uow = uow;
            _rng = rng;
            _refreshTokenConfiguration = configuration.GetSection("RefreshTokenOptions");
        }

        public RefreshToken Create(int userId)
        {
            var token = _rng.Generate();
            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_refreshTokenConfiguration["ExpiryMinutes"]));
            var refreshToken = new RefreshToken(userId, token, expiration);

            return refreshToken;
        }

        public async Task<AuthTokenDto> UseAsync(string refreshToken)
        {
            var token = await _uow.RefreshTokens.NotExpired().FirstOrDefaultAsync(x => x.Token == refreshToken);
            var user = token != null ? await _uow.Users.NotDeleted().Active().FirstOrDefaultAsync(x => x.Id == token.UserId) : null;
            if(token is null || user is null)
            {
                throw new InvalidRefreshTokenException();
            }
            var auth = _jwtProvider.Create(user);
            token.Update(_rng.Generate(), DateTime.UtcNow.AddMinutes(Convert.ToInt32(_refreshTokenConfiguration["ExpiryMinutes"])));
            auth.RefreshToken = token.Token;

            _uow.RefreshTokens.Update(token);
            await _uow.SaveChangesAsync();

            return auth;
        }
    }
}
