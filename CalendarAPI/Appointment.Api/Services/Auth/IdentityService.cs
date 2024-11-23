using Appointment.Api.Services.Auth.Interfaces;
using Appointment.Api.Services.Mail;
using Appointment.Data;
using Appointment.Domain.DTO.Identity;
using Appointment.Domain.Entities;
using Appointment.Domain.Exceptions.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Appointment.Domain.Services.Auth
{
    public class IdentityService : IIdentityService
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMailSendingService _mailSendingService;
        private readonly IPasswordService _passwordService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _uow;
        private readonly IRng _rng;

        public IdentityService(IRefreshTokenService refreshTokenService, IPasswordService passwordService, IJwtProvider jwtProvider, IUnitOfWork uow, IRng rng,IMailSendingService mailSendingService)
        {
            _refreshTokenService = refreshTokenService;
            _passwordService = passwordService;
            _jwtProvider = jwtProvider;
            _uow = uow;
            _rng = rng;
            _mailSendingService = mailSendingService;
        }
        /// <summary>
        /// Activate account, token is sent to user's email.
        /// Token is stored in user's resetToken field
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidCredentialsException"></exception>
        public async Task ActivateAccount(ActivateAccountDto dto)
        {
            if (string.IsNullOrEmpty(dto.Token))
            {
                throw new ArgumentNullException(nameof(dto.Token));
            }
            var decodedToken = WebEncoders.Base64UrlDecode(dto.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var user = await _uow.Users
                .NotDeleted().FirstOrDefaultAsync(x => x.ResetToken == normalToken);

            if(user == null)
            {
                throw new InvalidCredentialsException();
            }
            if(user.IsActive)
            {
                throw new UserAlreadyAcceptedException();
            }
            user.IsActive = true;
            user.ResetToken = null;
            await _uow.SaveChangesAsync();
        }

        public async Task<string> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = _uow.Users.NotDeleted().Active().FirstOrDefault(x => x.Email == dto.Email);
            if(user is null)
            {
                throw new InvalidCredentialsException(dto.Email);
            }
            var token = _rng.Generate();
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            await _mailSendingService.SendResetPasswordMail(dto.Email, token);

            user.ResetToken = token;
            await _uow.SaveChangesAsync();
            return validToken;
        }

        public async Task<AuthTokenDto> Refresh(RefreshTokenDto dto)
        {
            return await _refreshTokenService.UseAsync(dto.RefreshToken);
        }

        public async Task ResetPassword(ResetPasswordDto dto)
        {
            if(string.IsNullOrEmpty(dto.Token))
            {
                throw new EmptyTokenException();
            }
            var decodedToken = WebEncoders.Base64UrlDecode(dto.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var user = _uow.Users.NotDeleted().Active().FirstOrDefault(x => x.ResetToken == normalToken);

            if(user is null)
            {
                throw new InvalidCredentialsException();
            }

            _passwordService.CheckPasswordRequirements(dto.Password);

            if(_passwordService.IsValid(user.PasswordHash, dto.Password))
            {
                throw new NewPasswordTheSameAsOldException();
            }

            user.PasswordHash = _passwordService.Hash(dto.Password);
            user.ResetToken = null;
            user.RefreshTokens.Clear();

            await _uow.SaveChangesAsync();
        }

        public async Task SetPassword(int userId, SetPasswordDto dto)
        {
            var user = await _uow.Users.Include(u => u.RefreshTokens)
                .NotDeleted().Active().FirstOrDefaultAsync(x => x.Id == userId);

            if(user == null || !_passwordService.IsValid(user.PasswordHash, dto.OldPassword))
            {
                throw new InvalidCredentialsException();
            }

            _passwordService.CheckPasswordRequirements(dto.NewPassword);
            if(_passwordService.IsValid(user.PasswordHash, dto.NewPassword))
            {
                throw new NewPasswordTheSameAsOldException();
            }

            user.PasswordHash = _passwordService.Hash(dto.NewPassword);
            user.RefreshTokens.Clear();

            await _uow.SaveChangesAsync();
        }
        public async Task<AuthTokenDto> SignInAsync(SignInDto dto)
        {
            if (string.IsNullOrEmpty(dto.Login) || string.IsNullOrEmpty(dto.Password))
            {
                throw new InvalidCredentialsException(dto.Login);
            }
            var user = await _uow.Users.NotDeleted()
                        .FirstOrDefaultAsync(x => x.Login == dto.Login || x.Email == dto.Login);

            user = user switch
            {
                null => throw new InvalidCredentialsException(dto.Login),
                _ when !_passwordService.IsValid(user.PasswordHash, dto.Password) => throw new InvalidCredentialsException(dto.Login),
                _ when !user.IsActive => throw new UserNotActiveException(),
                _ => user
            };
            var auth = _jwtProvider.Create(user);
            var refreshToken = _refreshTokenService.Create(user.Id);
            auth.RefreshToken = refreshToken.Token;

            await _uow.RefreshTokens.AddAsync(refreshToken);
            await _uow.SaveChangesAsync();

            return auth;
        }

        public User SignUp(SignUpDto dto)
        {
            _passwordService.CheckPasswordRequirements(dto.Password);
            string passwordHash = _passwordService.Hash(dto.Password);
            var user = new User(dto.Login, dto.FirstName, dto.LastName, dto.Email, passwordHash, dto.Role, _uow.Users);

            var token = _rng.Generate();
            user.ResetToken = token;
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);
            _mailSendingService.SendConfirmationMail(user.Email, validToken);
            return user;
        }
    }
}
