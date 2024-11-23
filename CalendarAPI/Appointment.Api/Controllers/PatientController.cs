using Appointment.Api.Attributes;
using Appointment.Api.Auth;
using Appointment.Data;
using Appointment.Domain;
using Appointment.Domain.DTO;
using Appointment.Domain.DTO.Patient;
using Appointment.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Appointment.Api.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientController : BaseController
{
    private readonly IUnitOfWork _uow;

    public PatientController(IUnitOfWork uow, CalendarContext context)
        : base(context)
    {
        _uow = uow;
    }
    // GET: api/patients/{id}
    [HttpGet("{id}", Name = nameof(GetPatient))]
    [AuthorizeApi(Policies.User)]
    public async Task<ActionResult<PatientInfoDto>> GetPatient(int id)
    {
        var patient = await _uow.Patients.NotDeleted().WithIdAsync(id);
        if (patient is null)
        {
            return NotFound();
        }
        return Ok(patient.Present());
    }
    // POST: api/patients
    [HttpPost]
    [AuthorizeApi(Policies.User)]
    public async Task<ActionResult> Post(AddPatientDto dto)
    {
        Patient patient = new(dto, _uow.Patients);
        _uow.Patients.Add(patient);
        await _uow.SaveChangesAsync();
        return CreatedAtRoute(nameof(GetPatient), new { id = patient.Id }, patient);
    }
    // GET: api/patients/all
    [HttpGet("all")]
    [AuthorizeApi(Policies.User)]
    public async Task<ActionResult<GetAllItemsWithCountDto<PatientInfoDto>>> GetAll(int? page, int? pageSize, string? firstName, string? lastName, string? phone, string? email, bool? isNew)
    {
        var patients = _uow.Patients
            .NotDeleted()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .WithPhone(phone)
            .WithEmail(email);
        var presented = await patients.Page(page, pageSize).PresentedOnListAsync();
        return new GetAllItemsWithCountDto<PatientInfoDto>
        {
            Items = presented,
            Page = page ?? PatientSpecification.defaultPage,
            PageSize = pageSize ?? PatientSpecification.defaultPageSize,
            TotalCount = await patients.CountAsync()
        };
    }
    // DELETE: api/patients/{id}
    [HttpDelete("{id}")]
    [AuthorizeApi(Policies.Admin)]
    public async Task<ActionResult> Delete(int id)
    {
        var patient = await _uow.Patients.NotDeleted().WithIdAsync(id);
        if (patient is null)
        {
            return NotFound();
        }
        _uow.Patients.WithId(id).MarkAsDeleted();
        await _uow.SaveChangesAsync();
        return NoContent();
    }
    // PATCH: api/patients/{id}
    [HttpPatch("{id}")]
    [AuthorizeApi(Policies.User)]
    public async Task<ActionResult> Patch(int id, [FromBody] UpdatePatientDto dto)
    {
        var patient = await _uow.Patients.NotDeleted().WithIdAsync(id);
        if (patient is null)
        {
            return NotFound();
        }
        patient.Update(dto, _uow.Patients);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
