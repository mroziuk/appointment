using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Identity
{
    public class InvalidCredentialsException : DomainException
    {
        public string Login { get; }
        public InvalidCredentialsException(string login) : base($"Invalid credentials for user with login: {login}")
        {
            Login = login;
        }
        public InvalidCredentialsException() : base("Invalid credentials")
        {
            Login = null;
        }
    }
}
