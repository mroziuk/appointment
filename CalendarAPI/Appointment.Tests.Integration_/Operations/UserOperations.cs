using Appointment.Domain.DTO.Identity;
using Appointment.Domain.Entities;
using Appointment.Domain.Exceptions.Identity;
using Appointment.Domain.Exceptions.User;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Appointment.Tests.Integration_.Operations
{
    public class UserOperations : Operations
    {
        [TearDown]
        public void Dispose()
        {
            _uow.Users.RemoveRange(_uow.Users);
            _uow.Commit();
        }
        private async Task<User> SignUpAndActivate()
        {
            var registeredUser = _identityService.SignUp(
                new SignUpDto("user", "email@email.com", "User1234", "userFirstName", "userLastName", "user", null));
            registeredUser.IsActive = true;
            registeredUser.ResetToken = null;
            _uow.Users.Add(registeredUser);
            await _uow.SaveChangesAsync();
            return registeredUser;
        }
        private readonly SignUpDto _userDto = new SignUpDto("user", "email@email.com", "User1234", "userFirstName", "userLastName", "user", null);
        [Test]
        public async Task ShouldBeRegisteredCorrectly()
        {
            var registeredUser = _identityService.SignUp(
                new SignUpDto("user", "email@email.com", "User1234", "userFirstName", "userLastName", "user", null));

            await _uow.Users.AddAsync(registeredUser);
            await _uow.SaveChangesAsync();

            Assert.That(registeredUser.Created, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            Assert.That(registeredUser.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(registeredUser.Deleted, Is.Null);
            Assert.That(registeredUser.FirstName, Is.EqualTo("userFirstName"));
            Assert.That(registeredUser.LastName, Is.EqualTo("userLastName"));
            Assert.That(registeredUser.Email, Is.EqualTo("email@email.com"));
            Assert.That(_passwordService.IsValid(registeredUser.PasswordHash, "User1234"), Is.True);
            Assert.That(registeredUser.Role, Is.EqualTo("user"));
            Assert.That(registeredUser.IsActive, Is.False);

        }

        [Test]
        public async Task ShouldSignInCorrectly()
        {
            var user = await SignUpAndActivate();

            var authDto = await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));

            Assert.That(user.RefreshTokens.First().Token, Is.EqualTo(authDto.RefreshToken));
            Assert.That(authDto.Role, Is.EqualTo("user"));
            Assert.That(authDto.AccessToken, Is.Not.Null);
            Assert.That(authDto.RefreshToken, Is.Not.Null);
            Assert.That(DateTime.UnixEpoch.AddSeconds(authDto.Expires), Is.EqualTo(DateTime.UtcNow.AddMinutes(60)).Within(1).Minutes);
        }

        [Test]
        public async Task ShouldRefreshTokenCorrectly()
        {
            var user = await SignUpAndActivate();

            var authDto = await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));

            Assert.That(user.RefreshTokens.First().Token, Is.EqualTo(authDto.RefreshToken));

            var refreshedAuthDto = await _refreshTokenService.UseAsync(authDto.RefreshToken);

            Assert.That(refreshedAuthDto.RefreshToken, Is.EqualTo(user.RefreshTokens.First().Token));
            Assert.That(refreshedAuthDto.AccessToken, Is.Not.EqualTo(authDto.AccessToken));
            Assert.That(refreshedAuthDto.RefreshToken, Is.Not.EqualTo(authDto.RefreshToken));
            Assert.That(refreshedAuthDto.Role, Is.EqualTo(authDto.Role));
            Assert.That(DateTime.UnixEpoch.AddSeconds(refreshedAuthDto.Expires), Is.EqualTo(DateTime.UtcNow.AddMinutes(60)).Within(1).Minutes);
        }

        [Test]
        public void ShouldFailOnInvalidRefreshToken()
        {
            Assert.ThrowsAsync<InvalidRefreshTokenException>(async delegate
            {
                var user = await SignUpAndActivate();

                var authDto = await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));
                await _refreshTokenService.UseAsync(_refreshTokenService.Create(user.Id).Token);
            });
        }

        [Test]
        public void ShouldFailIfUserNotActive()
        {
            Assert.ThrowsAsync<UserNotActiveException>(async delegate
            {
                var notActiveUser = _identityService.SignUp(
                    new SignUpDto("user", "email@email.com", "User1234", "userFirstName", "userLastName", "user", null));

                await _uow.Users.AddAsync(notActiveUser);
                await _uow.SaveChangesAsync();

                await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));
            });
        }

        [Test]
        public async Task ShouldResetPasswordCorrectly()
        {
            var user = await SignUpAndActivate();

            var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });
            await _identityService.ResetPassword(new ResetPasswordDto
            { Token = token, Password = "12345User" });

            var authDto = await _identityService.SignInAsync(new SignInDto("email@email.com", "12345User"));
            Assert.That(authDto.Role, Is.EqualTo("user"));
            Assert.That(authDto.AccessToken, Is.Not.Null);
            Assert.That(authDto.RefreshToken, Is.Not.Null);
            Assert.That(DateTime.UnixEpoch.AddSeconds(authDto.Expires), Is.EqualTo(DateTime.UtcNow.AddMinutes(60)).Within(1).Minutes);
        }

        [Test]
        public void ShouldForgotPasswordFailOnInvalidCredentials()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(_userDto);

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "not_existing_user@email.com" });
            });
        }

        [Test]
        public void ShouldResetPasswordFailOnInvalidCredentials()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(_userDto);

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });
                await _identityService.ResetPassword(new ResetPasswordDto
                { Token = "invalid", Password = "12345User" });
            });
        }

        [Test]
        public void ShouldResetPasswordFailOnEmptyToken()
        {
            Assert.ThrowsAsync<EmptyTokenException>(async delegate
            {
                var user = await SignUpAndActivate();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });
                await _identityService.ResetPassword(new ResetPasswordDto
                { Token = null, Password = "12345User" });
            });
        }

        [Test]
        public void ShouldResetPasswordFailOnTheSamePasswordAsOld()
        {
            Assert.ThrowsAsync<NewPasswordTheSameAsOldException>(async delegate
            {
                var user = await SignUpAndActivate();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });
                await _identityService.ResetPassword(new ResetPasswordDto
                { Token = token, Password = "User1234" });
            });
        }

        [Test]
        public async Task ShouldRemoveRefreshTokensAfterResetPasswordCorrectly()
        {
            var user = await SignUpAndActivate();

            var authDto0 = await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));
            Assert.That(user.RefreshTokens.First().Token,Is.EqualTo(authDto0.RefreshToken));

            var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });
            await _identityService.ResetPassword(new ResetPasswordDto
            { Token = token, Password = "12345User" });

            Assert.That(user.RefreshTokens, Is.Empty);

            var authDto = await _identityService.SignInAsync(new SignInDto("email@email.com", "12345User"));
            Assert.Multiple(() =>
            {
                Assert.That(user.RefreshTokens.First().Token, Is.EqualTo(authDto.RefreshToken));

                Assert.That(authDto.Role, Is.EqualTo("user"));
                Assert.That(authDto.AccessToken, Is.Not.Empty);
                Assert.That(authDto.RefreshToken, Is.Not.Empty);
                Assert.That(DateTime.UnixEpoch.AddSeconds(authDto.Expires), Is.EqualTo(DateTime.UtcNow.AddMinutes(60)).Within(1).Minutes);
            });
        }

        [Test]
        public void ShouldSignInFailOnInvalidCredentials()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(
                    _userDto);

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                await _identityService.SignInAsync(new SignInDto("email@email.com", "invalid_password"));
            });
        }

        [Test]
        public void ShouldSignInFailIfUserDeleted()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(
                    _userDto);
                user.MarkAsDeleted();

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));
            });
        }

        [Test]
        public void ShouldForgotPasswordFailOnDeletedUser()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(
                    _userDto);
                user.MarkAsDeleted();

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });
            });
        }

        [Test]
        public void ShouldForgotPasswordFailOnNotActiveUser()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(
                    _userDto);
                user.IsActive = false;

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });
            });
        }

        [Test]
        public void ShouldResetPasswordFailOnDeletedUser()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(
                    _userDto);

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });

                user.MarkAsDeleted();
                _uow.Users.Update(user);
                await _uow.SaveChangesAsync();

                await _identityService.ResetPassword(new ResetPasswordDto
                { Token = token, Password = "12345User" });
            });
        }

        [Test]
        public void ShouldResetPasswordFailOnNotActiveUser()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(
                    _userDto);

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });

                user.IsActive = false;
                _uow.Users.Update(user);
                await _uow.SaveChangesAsync();

                await _identityService.ResetPassword(new ResetPasswordDto
                { Token = token, Password = "12345User" });
            });
        }

        [Test]
        public void ShouldFailOnRefreshDeletedUserToken()
        {
            Assert.ThrowsAsync<InvalidRefreshTokenException>(async delegate
            {
                var user = await SignUpAndActivate();

                var authDto = await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));

                user.MarkAsDeleted();
                _uow.Users.Update(user);
                await _uow.SaveChangesAsync();

                await _refreshTokenService.UseAsync(authDto.RefreshToken);
            });
        }

        [Test]
        public void ShouldFailOnRefreshNotActiveUserToken()
        {
            Assert.ThrowsAsync<InvalidRefreshTokenException>(async delegate
            {
                var user = await SignUpAndActivate();

                var authDto = await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));

                user.IsActive = false;
                _uow.Users.Update(user);
                await _uow.SaveChangesAsync();

                await _refreshTokenService.UseAsync(authDto.RefreshToken);
            });
        }

        [Test]
        public void ShouldFailOnRefreshExpiredToken()
        {
            Assert.ThrowsAsync<InvalidRefreshTokenException>(async delegate
            {
                var user = await SignUpAndActivate();

                var authDto = await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));

                var refresh = user.RefreshTokens.First();
                refresh.ExpirationTime = DateTime.UtcNow.AddMinutes(-1);

                _uow.RefreshTokens.Update(refresh);
                await _uow.SaveChangesAsync();

                await _refreshTokenService.UseAsync(authDto.RefreshToken);
            });
        }

        [Test]
        public async Task ShouldSetNewPasswordCorrectly()
        {
            var user = await SignUpAndActivate();

            var authDto0 = await _identityService.SignInAsync(new SignInDto("email@email.com", "User1234"));
            Assert.That(user.RefreshTokens.First().Token, Is.EqualTo(authDto0.RefreshToken));

            await _identityService.SetPassword(user.Id, new SetPasswordDto
            { OldPassword = "User1234", NewPassword = "12345User" });

            Assert.That(user.RefreshTokens, Is.Empty);

            var authDto = await _identityService.SignInAsync(new SignInDto("email@email.com", "12345User"));

            Assert.Multiple(() =>
            {
                Assert.That(user.RefreshTokens.First().Token, Is.EqualTo(authDto.RefreshToken));
                Assert.That(authDto.Role, Is.EqualTo("user"));
                Assert.That(authDto.AccessToken, Is.Not.Empty);
                Assert.That(authDto.RefreshToken, Is.Not.Empty);
                Assert.That(DateTime.UnixEpoch.AddSeconds(authDto.Expires), Is.EqualTo(DateTime.UtcNow.AddMinutes(60)).Within(1).Minutes);
            });
        }

        [Test]
        public void ShouldSetPasswordFailOnDeletedUser()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(
                    _userDto);

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });

                user.MarkAsDeleted();
                _uow.Users.Update(user);
                await _uow.SaveChangesAsync();

                await _identityService.SetPassword(user.Id, new SetPasswordDto
                { OldPassword = "User1234", NewPassword = "1234User" });
            });
        }

        [Test]
        public void ShouldSetPasswordFailOnNotActiveUser()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(
                    _userDto);

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });

                user.IsActive = false;
                _uow.Users.Update(user);
                await _uow.SaveChangesAsync();

                await _identityService.SetPassword(user.Id, new SetPasswordDto
                { OldPassword = "User1234", NewPassword = "1234User" });
            });
        }

        [Test]
        public void ShouldSetPasswordFailOnInvalidPassword()
        {
            Assert.ThrowsAsync<InvalidCredentialsException>(async delegate
            {
                var user = _identityService.SignUp(
                    _userDto);

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                await _identityService.SetPassword(user.Id, new SetPasswordDto
                { OldPassword = "invalid_pwd", NewPassword = "User1234" });
            });
        }

        [Test]
        public void ShouldSetPasswordFailOnTheSamePasswordAsOld()
        {
            Assert.ThrowsAsync<NewPasswordTheSameAsOldException>(async delegate
            {
                var user = await SignUpAndActivate();

                await _identityService.SetPassword(user.Id, new SetPasswordDto
                { OldPassword = "User1234", NewPassword = "User1234" });
            });
        }

        [Test]
        public void ShouldSetPasswordFailOnInvalidNewPassword()
        {
            Assert.ThrowsAsync<InvalidPasswordException>(async delegate
            {
                var user = await SignUpAndActivate();

                await _identityService.SetPassword(user.Id, new SetPasswordDto
                { OldPassword = "User1234", NewPassword = "User" });
            });
        }

        [Test]
        public void ShouldSignInFailOnInvalidPassword()
        {
            Assert.Throws<InvalidPasswordException>(delegate
            {
                _identityService.SignUp(
                    new SignUpDto("user", "email@email.com", "invalid","firstName", "lastName",  "user", null));
            });
        }

        [Test]
        public void ShouldResetPasswordFailOnInvalidPassword()
        {
            Assert.ThrowsAsync<InvalidPasswordException>(async delegate
            {
                var user = await SignUpAndActivate();

                var token = await _identityService.ForgotPassword(new ForgotPasswordDto { Email = "email@email.com" });

                await _identityService.ResetPassword(new ResetPasswordDto
                { Token = token, Password = "User" });
            });
        }
    }
}
