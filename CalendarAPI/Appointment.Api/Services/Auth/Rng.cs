using Appointment.Api.Services.Auth.Interfaces;
using System.Security.Cryptography;

namespace Appointment.Api.Services.Auth
{
    public class Rng : IRng
    {
        public string Generate(bool removeSpecialCharacters = true)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var bytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(bytes);
            var result = Convert.ToBase64String(bytes);

            return removeSpecialCharacters
                ? SpecialChars.Aggregate(result, (current, chars) => current.Replace(chars, string.Empty))
                : result;
        }
        private static readonly string[] SpecialChars = new[] { "/", "\\", "=", "+", "?", ":", "&" };
    }
}
