using Appointment.Domain.Entities;
using Appointment.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Appointment.Data;
using Appointment.Domain.DTO.Visit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Appointment.Api.Auth;
using Appointment.Api.Attributes;
using Appointment.Domain;

namespace Appointment.Api.Controllers
{
    [ApiController]
    [Route("api/visits")]
    public class VisitController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public VisitController(IUnitOfWork uow, CalendarContext context) : base(context)
        {
            _uow = uow;
        }
        // GET: api/visits/all
        [HttpGet("all")]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult<GetAllItemsWithCountDto<GetVisitDto>>> GetVisits(int? page, int? pageSize, int? userId, int? patientId, int? roomId,DateTime? dateFrom, DateTime? dateTo)
        {
            var visits = _uow.Visits.NotDeleted().WithPatient(patientId).WithRoom(roomId).WithUser(userId).WithDate(dateFrom, dateTo, true);
            var presentedVidits = await visits.Page(page, pageSize).PresentedOnListAsync();
            return new GetAllItemsWithCountDto<GetVisitDto>()
            {
                Items = presentedVidits,
                Page = page ?? VisitSpecification.defaultPage,
                PageSize = pageSize ?? VisitSpecification.defaultPageSize,
                TotalCount = await visits.CountAsync()
            };
        }
        // GET: api/visits/{id}
        [AuthorizeApi(Policies.User)]
        [HttpGet("{id}", Name = nameof(GetVisit))]
        public async Task<ActionResult<GetVisitDto>> GetVisit(int id)
        {
            var visit = await _uow.Visits.NotDeleted().WithIdAsync(id);
            if (visit == null)
            {
                return NotFound();
            }
            return Ok(visit.Present());
        }
        // POST: api/visits
        [HttpPost]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult> Post([FromBody] AddVisitDto dto)
        {
            if(dto.TherapistId != GetUserId() && !IsAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You cant create visit, that you do not own");
            }
            Visit visit = new(dto, _uow.Visits, _uow.Users, _uow.Patients, _uow.Rooms, _uow.Availabillities);
            _uow.Visits.Add(visit);
            await _uow.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetVisit), new { id = visit.Id }, new GetVisitDto(visit));
        }
        // DELETE: api/visits/{id}
        [HttpDelete("{id}")]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult> Delete(int id)
        {
            var visit = await _uow.Visits.NotDeleted().WithIdAsync(id);
            if (visit == null)
            {
                return NotFound();
            }
            if(visit.TherapistId != GetUserId() && !IsAdmin())  
            {
                return BadRequest("You cant delete visit, that you do not own");
            }
            _uow.Visits.WithId(id).MarkAsDeleted();
            await _uow.SaveChangesAsync();
            return NoContent();
        }
        // PATCH: api/visits/{id}
        [HttpPatch("{id}")]
        [AuthorizeApi(Policies.User)]
        public async Task<ActionResult> Patch(int id, [FromBody] UpdateVisitDto dto)
        {
            var visit = await _uow.Visits.NotDeleted().WithIdAsync(id);
            if (visit == null)
            {
                return NotFound();
            }
            if(visit.TherapistId != GetUserId() && !IsAdmin())
            {
                return BadRequest("You cant update visit, that you do not own");
            }
            visit.Update(dto, _uow.Visits, _uow.Users, _uow.Patients, _uow.Rooms, _uow.Availabillities);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
