using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Visit
{
    public class PatienthasVisitInthisDateException : DomainException
    {
        public PatienthasVisitInthisDateException(int patientId, DateTime dateFrom, DateTime dateTo)
            : base($"Patient {patientId} is has colliding visit(s) from {dateFrom} to {dateTo}.")
        {
            PatientId = patientId;
            DateFrom = dateFrom;
            DateTo = dateTo;
        }

        public int PatientId { get; }
        public DateTime DateFrom {get;}
        public DateTime DateTo { get;}

    }
}
