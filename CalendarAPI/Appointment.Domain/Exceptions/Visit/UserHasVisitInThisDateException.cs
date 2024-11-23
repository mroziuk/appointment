using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Visit
{
    public class UserHasVisitInThisDateException : DomainException
    {
        public UserHasVisitInThisDateException(int userId, DateTime dateFrom, DateTime dateTo)
            : base($"User {userId} has colliding visit(s) from {dateFrom} to {dateTo}.")
        {
            UserId = userId;
            DateFrom = dateFrom;
            DateTo = dateTo;
        }

        public int UserId { get; }
        public DateTime DateFrom { get; }
        public DateTime DateTo { get; }
    }
}
