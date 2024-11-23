using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Visit
{
    public class AddVisitDto
    {
        public int TherapistId { get; set; }
        public int PatientId { get; set; }
        public int RoomId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool IsRecurring { get; set; }
    }
}
