using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Patient
{
    public class EmptyPhoneNumberException : DomainException
    {
        public EmptyPhoneNumberException() :base ("Phone number cannot be empty") { }
    }
}
