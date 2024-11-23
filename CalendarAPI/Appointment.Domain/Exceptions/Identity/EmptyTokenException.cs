using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Identity
{
    public class EmptyTokenException : DomainException
    {
        public EmptyTokenException() : base("Token cannot be empty")
        {
        }
    }
}
