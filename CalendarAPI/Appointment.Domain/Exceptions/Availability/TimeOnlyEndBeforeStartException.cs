using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Availability
{
    public class TimeOnlyEndBeforeStartException : DomainException
    {
        public TimeOnlyEndBeforeStartException(TimeOnly start, TimeOnly end) : base($"End date or time {end} cannot be before start date or time {start}.")
        {
            Start = start;
            End = end;
        }
        public TimeOnly Start { get; }
        public TimeOnly End { get; }
    }
}
