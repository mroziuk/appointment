using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Availability
{
    public class AvailabilityOverlapException : DomainException
    {
        public AvailabilityOverlapException(int? availabilityId, TimeOnly start, TimeOnly end) : base($"Availability overlaping with availability {start} : {end}")
        {
            this.AvailabilityId = availabilityId;
            Start = start;
            End = end;
        }
        public int? AvailabilityId { get; }
        public TimeOnly Start { get; }
        public TimeOnly End { get;}
    }
}
