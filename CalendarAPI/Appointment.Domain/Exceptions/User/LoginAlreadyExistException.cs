using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.User
{
    public class LoginAlreadyExistException : DomainException
    {
        public LoginAlreadyExistException(string login) :base($"Email {login} already in use")
        {
            Login = login;
        }

        public string Login { get; }
    }
}
