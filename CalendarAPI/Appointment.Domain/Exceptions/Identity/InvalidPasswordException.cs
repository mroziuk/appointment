using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Identity
{
    public class InvalidPasswordException : DomainException
    {
        public InvalidPasswordException() : base("Password must contain at least 8 characters, 1 number, 1 upper case and 1 lower case.") { }
    }
}
