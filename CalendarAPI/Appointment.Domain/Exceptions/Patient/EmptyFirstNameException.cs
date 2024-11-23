using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Patient
{
    public class EmptyFirstNameException : DomainException
    {
        public EmptyFirstNameException() :base("First Name cannot be empty") { }
    }
}
