using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Availability
{
    public class DateTimeEndBeforeStartException : DomainException
    {
        public DateTimeEndBeforeStartException(DateTime start, DateTime end) : base($"End date or time {end} cannot be before start date or time {start}.")
        {
            Start = start;
            End = end;
        }
        public DateTime Start { get;}
        public DateTime End { get;}
    }
}
