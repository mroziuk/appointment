using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Identity
{
    public class NewPasswordTheSameAsOldException : DomainException
    {
        public NewPasswordTheSameAsOldException() : base("New password cannot be the same as the old one")
        {
        }
    }
}
