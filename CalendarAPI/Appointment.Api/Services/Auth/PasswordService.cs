using Appointment.Common.Utils;
using Appointment.Domain.Exceptions.Identity;
using Appointment.Api.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;

namespace Appointment.Domain.Services.Auth
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHasher<IPasswordService> _passwordHasher;

        public PasswordService(IPasswordHasher<IPasswordService> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public void CheckPasswordRequirements(string password)
        {
            Require.NotEmpty(password).OrError(new InvalidPasswordException());
            Require.IsTrue(
                password.Length >= 8 &&
                password.Any(char.IsUpper) &&
                password.Any(char.IsLower) &&
                password.Any(char.IsNumber)
            ).OrError(new InvalidPasswordException());
        }

        public string Hash(string password) => _passwordHasher.HashPassword(this, password);

        public bool IsValid(string hash, string password) => _passwordHasher.VerifyHashedPassword(this, hash, password) != PasswordVerificationResult.Failed;
    }
}
