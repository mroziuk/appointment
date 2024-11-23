namespace Appointment.Api.Services.Auth.Interfaces
{
    public interface IRng
    {
        string Generate(bool removeSpecialCharacters = true);
    }
}
