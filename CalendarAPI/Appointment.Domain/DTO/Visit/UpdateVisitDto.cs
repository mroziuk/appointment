using Appointment.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Visit
{
    public record UpdateVisitDto
    {
        public PropertyUpdater<int> TherapistId { get; init; }
        public PropertyUpdater<int> PatientId { get; init; }
        public PropertyUpdater<int> RoomId { get; init; }
        public PropertyUpdater<DateTime> DateStart { get; init; }
        public PropertyUpdater<DateTime> DateEnd { get; init; }
        public PropertyUpdater<bool> IsRecurring { get; init; }
    }
}
