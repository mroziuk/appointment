using Appointment.Domain.DTO.Identity;
using Appointment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Api.Services.Auth.Interfaces
{
    public interface IRefreshTokenService
    {
        RefreshToken Create(int userId);
        Task<AuthTokenDto> UseAsync(string refreshToken);
    }
}
