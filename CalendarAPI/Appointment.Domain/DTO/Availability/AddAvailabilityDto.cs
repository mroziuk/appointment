using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Availability
{
    public class AddAvailabilityDto
    {
        public int UserId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly Start {get; set; }
        public TimeOnly End { get; set; }
        public DateOnly ActiveFrom { get; set; }
        public DateOnly ActiveTo { get; set;}
    }
}
