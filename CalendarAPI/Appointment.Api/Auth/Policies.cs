using Appointment.Domain.Entities.Identity;

namespace Appointment.Api.Auth
{
    public static class Policies
    {
        public const string Admin = Role.Admin;
        public const string User = Role.User;
        public const string SuperAdmin = Role.SuperAdmin;
    }
}
