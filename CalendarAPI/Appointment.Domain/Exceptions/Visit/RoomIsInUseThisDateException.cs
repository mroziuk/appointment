using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Visit
{
    public class RoomIsInUseThisDateException : DomainException
    {
        public RoomIsInUseThisDateException(int roomId, DateTime dateFrom, DateTime dateTo)
            : base($"Room {roomId} has colliding visit(s) from {dateFrom} to {dateTo}.")
        {
            RoomId = roomId;
            DateFrom = dateFrom;
            TimeTo = dateTo;
        }

        public int RoomId { get; }
        public DateTime DateFrom { get; }
        public DateTime TimeTo { get; }
    }
}
