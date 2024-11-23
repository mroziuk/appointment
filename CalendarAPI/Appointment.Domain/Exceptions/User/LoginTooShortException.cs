using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.User
{
    internal class LoginTooShortException : DomainException
    {
        public LoginTooShortException(string login) : base($"Login {login} is too short.")
        {
            Login = login;
        }

        public string Login { get; }
    }
}
