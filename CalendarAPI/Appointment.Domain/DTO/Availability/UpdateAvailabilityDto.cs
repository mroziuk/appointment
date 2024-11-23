using Appointment.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Availability
{
    public record UpdateAvailabilityDto
    {
        public UpdateAvailabilityDto(PropertyUpdater<int> userId, PropertyUpdater<int> dayOfWeek, PropertyUpdater<TimeOnly> start, PropertyUpdater<TimeOnly> end, PropertyUpdater<DateOnly> activeFrom, PropertyUpdater<DateOnly> activeTo)
        {
            UserId = userId;
            DayOfWeek = dayOfWeek;
            Start = start;
            End = end;
            ActiveFrom = activeFrom;
            ActiveTo = activeTo;
        }
        public UpdateAvailabilityDto() { }
        public PropertyUpdater<int> UserId { get; init; }
        public PropertyUpdater<int> DayOfWeek { get; init; }
        public PropertyUpdater<TimeOnly> Start { get; init; }
        public PropertyUpdater<TimeOnly> End { get; init; }
        public PropertyUpdater<DateOnly> ActiveFrom { get; init; }
        public PropertyUpdater<DateOnly> ActiveTo { get; init; }

    }
}
