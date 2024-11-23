using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Identity
{
    public class AuthTokenDto
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; set; }
        public string Role { get; init; }
        public long Expires { get; init; }

    }
}
