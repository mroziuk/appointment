using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Patient
{
    public class LastNameTooLongException : DomainException
    {
        public LastNameTooLongException(string lastName) : base($"last name: {lastName} is too long.")
        {
            LastName = lastName;
        }

        public string LastName { get; }

    }
}
