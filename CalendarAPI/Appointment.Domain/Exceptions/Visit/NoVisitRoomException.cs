using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Visit
{
    public class NoVisitRoomException : DomainException
    {
        public int RoomId { get; }
        public NoVisitRoomException(int roomId) : base($"Room with {roomId} doesnt exist.")
        {
            RoomId = roomId;
        }
    }
}
