using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Availability
{
    public class NoAvailabilityActiveFromDateException : DomainException
    {
        public NoAvailabilityActiveFromDateException() : base("Availability active from date is empty") { }
    }
}
