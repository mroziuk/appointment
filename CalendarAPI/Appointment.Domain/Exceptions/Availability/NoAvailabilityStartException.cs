using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Availability
{
    public class NoAvailabilityStartException : DomainException
    {
        public NoAvailabilityStartException() : base("Availability start is empty") { }
    }
}
