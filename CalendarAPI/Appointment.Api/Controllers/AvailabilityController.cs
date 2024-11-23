
using Appointment.Data;
using Appointment.Domain;
using Appointment.Domain.DTO;
using Appointment.Domain.DTO.Availability;
using Appointment.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Appointment.Api.Attributes;
using Appointment.Api.Auth;

namespace Appointment.Api.Controllers
{
    [ApiController]
    [Route("api/availability")]
    public class AvailabilityController : BaseController
    {
        private readonly IUnitOfWork _uow;
        public AvailabilityController(IUnitOfWork uow, CalendarContext context)
            : base(context)
        {
            _uow = uow;
        }
        // GET: api/availability/{id}
        [HttpGet("{id}", Name = nameof(GetAvailability))]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult<GetAvailabilityDto>> GetAvailability(int id)
        {
            var availability = await _uow.Availabillities.NotDeleted().WithIdAsync(id);
            if (availability == null)
            {
                return NotFound();
            }
            return Ok(availability.Present());
        }
        // GET: api/availability/all
        [HttpGet("all")]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult<GetAllItemsWithCountDto<GetAvailabilityDto>>> GetAvailabilities(int? page, int? pageSize, int? userId)
        {
            var avaliabilities =
                 _uow.Availabillities
                .NotDeleted()
                .WithUser(userId);
            var presentedAvailabilities = await avaliabilities.Page(page, pageSize).PresentedOnListAsync();
            var result = new GetAllItemsWithCountDto<GetAvailabilityDto>()
            {
                Items = presentedAvailabilities,
                Page = page ?? AvailabillitySpecification.defaultPage,
                PageSize = pageSize ?? AvailabillitySpecification.defaultPageSize,
                TotalCount = await avaliabilities.CountAsync()
            };
            return Ok(result);
        }
        // POST: api/availability
        [HttpPost]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult> Post([FromBody] AddAvailabilityDto dto)
        {
            if(dto.UserId != GetUserId() && !IsAdmin())
            {
                return BadRequest();
            }
            var availability = new Availabillity(dto, _uow.Users, _uow.Availabillities);
            await _uow.Availabillities.AddAsync(availability);
            await _uow.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetAvailability), new { id = availability.Id }, availability);
        }
        //DELETE: api/availability/{id}
        [HttpDelete("{id}")]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult> Delete(int id)
        {
            var availability = await _uow.Availabillities.SingleOrDefaultAsync(x => x.Id == id);
            if (availability == null)
            {
                return NotFound();
            }
            if (availability.UserId != GetUserId() && !IsAdmin())
            {
                return BadRequest();
            }
            _uow.Availabillities.Remove(availability);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
        // PATCH: api/availability/{id}
        [HttpPatch("{id}")]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult> Patch(int id, [FromBody] UpdateAvailabilityDto dto)
        {
            var availability = await _uow.Availabillities.NotDeleted().WithIdAsync(id);
            if (availability == null)
            {
                return NotFound();
            }
            if (availability.UserId != GetUserId() && !IsAdmin())
            {
                return BadRequest();
            }
            availability.Update(dto, _uow.Users, _uow.Availabillities);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
