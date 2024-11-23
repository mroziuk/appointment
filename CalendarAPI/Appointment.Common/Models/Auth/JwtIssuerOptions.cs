using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Common.Models.Auth
{
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }

        public string IssuerSigningKey { get; set; }

        public string Algorithm { get; set; }

        public string ExpiryMinutes { get; set; }
    }
}
