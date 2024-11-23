using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Availability
{
    public class NoAvailabilityUserException : DomainException
    {
        public int? UserId { get; }
        public NoAvailabilityUserException(int? userId) : base($"User with id {userId} doesnt exist.")
        {
            UserId = userId;
        }
    }
}
