using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Visit
{
    public class NoVaildAvailabilityForVisitException : DomainException
    {
        public int UserId { get; }
        public DateTime DateTime { get; }

        public NoVaildAvailabilityForVisitException(DateTime dateTime, int id) : base($"Availability for user {id} and date {dateTime} doesnt exist")
        {
            DateTime = dateTime;
            UserId = id;
        }
    }
}
