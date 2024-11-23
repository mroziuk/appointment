using Appointment.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO
{
    public record UpdateUserDto
    {
        public PropertyUpdater<string> Login { get; init; }
        public PropertyUpdater<string> FirstName { get; init; }
        public PropertyUpdater<string> LastName { get; init; }
        public PropertyUpdater<string> Email { get; init; }
        public PropertyUpdater<string> Phone { get; init; }
        public PropertyUpdater<string> Role { get; init; }
        public PropertyUpdater<bool> IsActive { get; set; }

    }
}
