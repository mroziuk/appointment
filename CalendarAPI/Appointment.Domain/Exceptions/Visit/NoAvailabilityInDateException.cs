using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Visit
{
    public class NoAvailabilityInDateException : DomainException
    {
        public NoAvailabilityInDateException(int userId, DateTime dateFrom, DateTime dateTo)
            : base($"No availability found for user {userId} from {dateFrom} to {dateTo}.")
        {
            UserId = userId;
            this.dateFrom = dateFrom;
            this.dateTo = dateTo;
        }

        public int UserId { get; }
        public DateTime dateFrom { get; }
        public DateTime dateTo { get; }

    }
}
