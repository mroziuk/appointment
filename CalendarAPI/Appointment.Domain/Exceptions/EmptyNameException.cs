using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions
{
    public class EmptyNameException : DomainException
    {
        public EmptyNameException() : base("Name is empty") { }
    }
}
