using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.Room
{
    public record GetRoomDto
    {
        public GetRoomDto(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public GetRoomDto() { }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
