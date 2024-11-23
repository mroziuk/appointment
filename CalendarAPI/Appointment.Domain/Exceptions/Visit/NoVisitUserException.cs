using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Visit
{
    public class NoVisitUserException : DomainException
    {
        public NoVisitUserException(int userId) : base($"User with id {userId} doesnt exist.")
        {
            UserId = userId;
        }

        public int UserId { get; }
    }
}
