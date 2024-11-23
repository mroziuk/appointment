using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Identity
{
    public record ResetPasswordDto
    {
        public string Token { get; init; }
        public string Password { get; init; }
    }
}
