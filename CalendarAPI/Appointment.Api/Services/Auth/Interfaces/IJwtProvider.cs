using Appointment.Domain.DTO.Identity;
using Appointment.Domain.Entities;
using System.Security.Claims;

namespace Appointment.Api.Services.Auth.Interfaces
{
    public interface IJwtProvider
    {
        AuthTokenDto Create(User user, List<Claim> claims = null);
        AuthTokenDto Create(string id, string role, List<Claim> claims = null);
    }
}
