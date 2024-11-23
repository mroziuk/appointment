using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Patient
{
    public class EmptyLastNameException : DomainException
    {
        public EmptyLastNameException() :base("Last name cannot be empty") { }
    }
}
