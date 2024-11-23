using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Room
{
    public class RoomNameAlreadyExistsException : DomainException
    {
        public RoomNameAlreadyExistsException(string name) : base($"Name {name} already exists")
        {
            Name = name;
        }
        public string Name { get; }
    }
}
