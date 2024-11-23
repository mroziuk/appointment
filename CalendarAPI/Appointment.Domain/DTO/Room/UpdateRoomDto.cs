using Appointment.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Room
{
    public record UpdateRoomDto
    {
        public PropertyUpdater<string> Name { get; init; }
    }
}
