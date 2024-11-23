using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public virtual string ErrorCode { get; }
        public DomainException(string message) : base(message)
        {

        }
    }
}
