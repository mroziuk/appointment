using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Identity
{
    public class SignUpDto
    {
        public SignUpDto(string login, string email, string password, string firstName, string lastName,string role, string? phone)
        {
            Login = login;
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            Phone = phone;
        }

        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string? Phone { get; set; }
    }
}
