using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Availability
{
    public class DateOnlyEndBeforeStartException : DomainException
    {
        public DateOnlyEndBeforeStartException(DateOnly start, DateOnly end) : base($"End date or time {end} cannot be before start date or time {start}.")
        {
            this.Start = start;
            this.End = end;
        }
        public DateOnly Start { get; }
        public DateOnly End { get; }
    }
}
