using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.User
{
    public class InvalidEmailException : DomainException
    {
        public string Email { get; }
        public InvalidEmailException(string email) : base($"Email {email} is invalid")
        {
            Email = email;
        }
    }
}
