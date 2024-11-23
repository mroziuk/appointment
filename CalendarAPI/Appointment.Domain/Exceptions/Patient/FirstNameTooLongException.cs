using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Patient
{
    public class FirstNameTooLongException : DomainException
    {
        public FirstNameTooLongException(string firstName) : base($"First name: {firstName} is too long.")
        {
            FirstName = firstName;
        }

        public string FirstName { get; }

    }
}
