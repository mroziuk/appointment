using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Room
{
    public class RoomNameTooLongException: DomainException
    {
        public RoomNameTooLongException(string name) : base($"Name {name} is too long")
        {
            Name = name;
        }

        public string Name { get; }

    }
}
