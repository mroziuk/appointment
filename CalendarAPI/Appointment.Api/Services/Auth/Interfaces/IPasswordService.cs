using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Api.Services.Auth.Interfaces
{
    public interface IPasswordService
    {
        void CheckPasswordRequirements(string password);
        bool IsValid(string hash, string password);
        string Hash(string password);
    }
}
