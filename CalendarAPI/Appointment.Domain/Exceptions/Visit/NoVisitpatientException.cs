using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Visit
{
    public class NoVisitpatientException : DomainException
    {
        public NoVisitpatientException(int patientId) : base($"Patient with id {patientId} doesnt exist")
        {
            PatientId = patientId;
        }
        public int PatientId { get; }
    }
}
