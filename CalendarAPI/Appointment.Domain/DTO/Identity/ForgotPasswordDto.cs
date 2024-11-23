using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Identity
{
    public record ForgotPasswordDto
    {
        public string Email { get; init; }
    }
}
