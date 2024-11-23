using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Entities.Identity
{
    public static class Role
    {
        public const string User = "user";
        public const string Admin = "admin";
        public const string SuperAdmin = "superadmin";
        public static bool IsValidForRegistration(string role)
        {
            return role == Role.User || role == Role.Admin;
        }
        public static bool IsValid(string role)
        {
            if(string.IsNullOrEmpty(role))
            {
                return false;
            }
            role = role.ToLowerInvariant();

            return role == Role.User || role == Role.Admin;
        }
    }
}
