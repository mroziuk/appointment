using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Availability
{
    public class NoAvailabilityEndException : DomainException
    {
        public NoAvailabilityEndException() : base("Availability end is empty") { }
    }
}
