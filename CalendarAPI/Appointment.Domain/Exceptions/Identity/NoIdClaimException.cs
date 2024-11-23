using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.Identity
{
    public class NoIdClaimException : DomainException
    {
        public override string ErrorCode { get; } = nameof(NoIdClaimException);

        public NoIdClaimException() : base("User doesn't have ID claim.") { }
    }
}
