using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions
{
    public class InvalidPaginationException : DomainException
    {
        public override string ErrorCode { get; } = nameof(InvalidPaginationException);
        public int? Page { get; }
        public int? PageSize { get; }

        public InvalidPaginationException(int? page, int? pageSize) : base($"Invalid page {page} or pageSize {pageSize}.")
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
