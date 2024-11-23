using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Identity
{
    public record ActivateAccountDto
    {
        public ActivateAccountDto(string token)
        {
            Token = token;
        }
        public ActivateAccountDto()
        {
            
        }
        public string Token { get; init; }
    }
}
