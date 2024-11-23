using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Identity
{
    public class UserAlreadyAcceptedException : DomainException
    {
        public UserAlreadyAcceptedException() : base("User is already accepted.")
        {
        }
    }
}
