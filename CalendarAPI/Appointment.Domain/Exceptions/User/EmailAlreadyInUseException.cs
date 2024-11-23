using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.User
{
    public class EmailAlreadyInUseException : DomainException
    {
        public EmailAlreadyInUseException(string email) : base($"Email {email} already in use.")
        {
            Email = email;
        }

        public string Email { get; }

    }
}
