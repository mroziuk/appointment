using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO
{
    public record AddUserDto
    {
        public string Login { get; set; }
        public string Email { get; init; }
        public string Password { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string? Phone { get; init; }
        public string Role { get; init; }
    }
}
