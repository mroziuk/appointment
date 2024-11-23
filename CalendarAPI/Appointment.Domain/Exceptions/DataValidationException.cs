using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions
{
    public class DataValidationException : DomainException
    {
        public string Property { get; }
        public DataValidationException(string property) : base($"Invalid data for property: {property}")
        {
            Property = property;
        }
    }
}
