using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.User
{
    public class LoginTooLongException : DomainException
    {
        public string Login { get; }
        public LoginTooLongException(string login) : base($"Login {login} is too long")
        {
            Login = login;
        }
    }
}
