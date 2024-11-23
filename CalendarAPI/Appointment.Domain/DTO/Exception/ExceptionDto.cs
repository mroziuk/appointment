using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Exception
{
    public record ExceptionDto
    {
        public ExceptionDto(int code, string name, string message)
        {
            Code = code;
            Name = name;
            Message = message;
        }
        public int Code { get; init; }
        public string Name { get; init; }
        public string Message { get; init; }
    }
}
