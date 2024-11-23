using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Visit
{
    public class DateEndBeforeStartException : DomainException
    {
        public DateTime DateStart { get; }
        public DateTime DateEnd { get; }

        public DateEndBeforeStartException(DateTime dateStart, DateTime dateEnd)
            : base($"DateEnd: {dateEnd} is before DateStart: {dateStart}")
        {
            DateStart = dateStart;
            DateEnd = dateEnd;
        }
    }
}
