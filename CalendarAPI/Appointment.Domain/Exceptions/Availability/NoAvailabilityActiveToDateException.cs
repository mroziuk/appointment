using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Availability
{
    public class NoAvailabilityActiveToDateException : DomainException
    {
        public NoAvailabilityActiveToDateException() : base("Availability active to date is empty") { }
    }
}
