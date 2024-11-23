using Appointment.Domain.DTO.Identity;
using Appointment.Domain.Entities;

namespace Appointment.Api.Services.Auth.Interfaces;

public interface IIdentityService
{
    User SignUp(SignUpDto userDto);
    Task<AuthTokenDto> SignInAsync(SignInDto userDto);
    Task<string> ForgotPassword(ForgotPasswordDto dto);
    Task SetPassword(int userId, SetPasswordDto dto);
    Task ResetPassword(ResetPasswordDto dto);
    Task ActivateAccount(ActivateAccountDto dto);
    Task<AuthTokenDto> Refresh(RefreshTokenDto dto);
}
