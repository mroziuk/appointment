using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Patient
{
    public class PatientAlreadyExistException : DomainException
    {
        public PatientAlreadyExistException(string firstName, string lastName) : base($"User with name {firstName} {lastName} already exist in the database")
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string LastName { get; }

    }
}
