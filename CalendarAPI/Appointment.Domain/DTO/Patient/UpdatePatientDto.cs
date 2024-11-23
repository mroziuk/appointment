using Appointment.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Patient
{
    public class UpdatePatientDto
    {
        public PropertyUpdater<string> FirstName { get; init; }
        public PropertyUpdater<string> LastName { get; init; }
        public PropertyUpdater<string> Phone { get; init; }
        public PropertyUpdater<string> Email { get; init; }
    }
}
