using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Availability
{
    public class GetAvailabilityDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeOnly Start {  get; set; }
        public TimeOnly End { get; set; }
        public DateTime ActiveFrom {  get; set; }
        public DateTime ActiveTo { get; set;}
    }
}
