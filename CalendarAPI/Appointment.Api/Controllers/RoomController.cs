using Appointment.Api.Attributes;
using Appointment.Api.Auth;
using Appointment.Api.Services;
using Appointment.Data;
using Appointment.Domain;
using Appointment.Domain.DTO;
using Appointment.Domain.DTO.Room;
using Appointment.Domain.DTO.Visit;
using Appointment.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Appointment.Api.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public RoomController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        // GET: api/rooms/{id}
        [AuthorizeApi(Policies.User)]
        [HttpGet("{id}", Name = nameof(GetRoom))]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _uow.Rooms.NotDeleted().SingleOrDefaultAsync(r => r.Id == id);
            if(room == null)
            {
                return NotFound();
            }
            return Ok(room);
        }
        [AuthorizeApi(Policies.User)]
        [HttpGet("all")]
        public async Task<GetAllItemsWithCountDto<GetRoomDto>> GetAll(int? page, int? pageSize, string? name)
        {
            var rooms = _uow.Rooms.WithName(name).NotDeleted();
            var roomsPresented = await rooms.Page(page, pageSize).PresentedOnListAsync();
            return new GetAllItemsWithCountDto<GetRoomDto>
            {
                Page = page ?? RoomSpecification.defaultPage,
                PageSize = pageSize ?? RoomSpecification.defaultPageSize,
                TotalCount = await rooms.CountAsync(),
                Items = roomsPresented,
            };
        }
        [HttpPost]
        [AuthorizeApi(Policies.Admin)]
        public async Task<ActionResult> Post(AddRoomDto dto)
        {
            Room room = new(dto, _uow.Rooms);
            _uow.Rooms.Add(room);
            await _uow.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetRoom), new { id = room.Id }, room);
        }
        [HttpPatch("{id}")]
        [AuthorizeApi(Policies.Admin)]
        public async Task<ActionResult> Patch(int id, [FromBody] UpdateRoomDto dto)
        {
            Room? room = await _uow.Rooms.NotDeleted().WithIdAsync(id);
            if(room is null)
            {
                return NotFound();
            }
            room.Update(dto, _uow.Rooms);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        [AuthorizeApi(Policies.Admin)]
        public async Task<ActionResult> Delete(int id)
        {
            var room = await _uow.Rooms.NotDeleted().WithIdAsync(id);
            if(room is null)
            {
                return NotFound();
            }
            room.MarkAsDeleted();
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
