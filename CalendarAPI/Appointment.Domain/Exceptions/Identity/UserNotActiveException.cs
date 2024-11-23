using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Identity
{
    public class UserNotActiveException : DomainException
    {
        public override string ErrorCode { get; } = nameof(UserNotActiveException);

        public UserNotActiveException() : base("User is not active.")
        {

        }
    }
}
