using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Identity
{
    public record SetPasswordDto
    {
        public string OldPassword { get; init; }
        public string NewPassword { get; init; }
    }
}
