using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Patient
{
    public class InvalidPhoneNumberException : DomainException
    {
        public string PhoneNumber { get; set; }
        public InvalidPhoneNumberException(string phoneNumber) :base($"Phone Number {phoneNumber} is invalid")
        {
            PhoneNumber = phoneNumber;
        }
    }
}
