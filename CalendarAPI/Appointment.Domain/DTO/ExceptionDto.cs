using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO
{
    public class ExceptionDto
    {
        public int Code { get; }
        public string Name { get; }
        public string Message { get; }
        public ExceptionDto(int code, string name, string message)
        {
            Code = code;
            Name = name;
            Message = message;
        }

    }
}
