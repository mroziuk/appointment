using Appointment.Common.Utils;
using Appointment.Domain.DTO.Patient;
using Appointment.Domain.DTO.User;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.Patient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Appointment.Domain.Entities;

public class Patient : AuditableEntity
{
    public Patient() { }
    public Patient(AddPatientDto dto, DbSet<Patient> patients)
    {
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        Phone = dto.Phone;
        Email = string.Empty;
        Validate(patients);
    }
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public List<Visit> Visits { get; set; }
    public bool IsNew { get; set; }
    public bool IsRecurring { get; set; }

    public void MarkAsDeleted()
    {
        Deleted = DateTime.UtcNow;
    }
    public void Update(UpdatePatientDto dto, DbSet<Patient>? patients = null)
    {
        if(dto.FirstName.NeedsUpdate) FirstName = dto.FirstName.NewValue?.Trim();
        if(dto.LastName.NeedsUpdate) LastName = dto.LastName.NewValue?.Trim();
        if(dto.Phone.NeedsUpdate) Phone = dto.Phone.NewValue?.Trim();
        if(dto.Email.NeedsUpdate) Email = dto.Email.NewValue?.Trim();

        Validate(patients);
        LastModified = DateTime.UtcNow;
    }
    public PatientInfoDto Present()
    {
        return new PatientInfoDto   (
            Id,
            FirstName,
            LastName,
            Phone);
    }
    public void Validate(DbSet<Patient>? patients)
    {
        if(patients is not null) Require.IsEmpty(patients.NotDeleted().WithExactName(FirstName, LastName).Where(p => p.Id != Id)).OrError(new PatientAlreadyExistException(FirstName, LastName));
        Require.NotEmpty(FirstName).OrError(new EmptyFirstNameException());
        Require.NotEmpty(LastName).OrError(new EmptyLastNameException());
        Require.NotEmpty(Phone).OrError(new EmptyPhoneNumberException());
        Require.IsFalse(FirstName.Length > 128).OrError(new FirstNameTooLongException(FirstName));
        Require.IsFalse(LastName.Length > 128).OrError(new LastNameTooLongException(LastName));
        Require.IsTrue(PhoneUtils.IsPhoneNumber(Phone)).OrError(new InvalidPhoneNumberException(Phone));
    }
}

public static class PatientSpecification
{
    public const int defaultPage = 1;
    public const int defaultPageSize = 10;

    public static async Task<List<PatientInfoDto>> PresentedOnListAsync(this IQueryable<Patient> items)
    {
        return await items.Select(item => item.Present()).ToListAsync();
    }
    public static IQueryable<Patient> Page(this IQueryable<Patient> items, int? page, int? pageSize)
    {
        page ??= defaultPage;
        pageSize ??= defaultPageSize;

        if(pageSize <= 0 || page.Value <= 0)
        {
            throw new InvalidPaginationException(page, pageSize);
        }
        return items.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
    }
    public static Patient WithId(this IQueryable<Patient> items, int id)
    {
        return items.FirstOrDefault(i => i.Id == id);
    }
    public static async Task<Patient> WithIdAsync(this IQueryable<Patient> items, int id)
    {
        return await items.FirstOrDefaultAsync(i => i.Id == id);
    }
    public static IQueryable<Patient> WithName(this IQueryable<Patient> items, string? name)
    {
        if(name == null) return items;
        return items.Where(i => i.LastName != null && (
            i.LastName.ToLower().Contains(name.ToLower()) ||
            i.FirstName.ToLower().Contains(name.ToLower())));
    }
    public static IQueryable<Patient> WithFirstName(this IQueryable<Patient> items, string? firstName)
    {
        if(firstName is null) return items;
        return items.Where(i => i.FirstName.ToLower().Contains(firstName.ToLower()));
    }
    public static IQueryable<Patient> WithLastName(this IQueryable<Patient> items, string? lastName)
    {
        if(lastName is null) return items;
        return items.Where(i => i.LastName.ToLower().Contains(lastName.ToLower()));
    }
    public static IQueryable<Patient> WithPhone(this IQueryable<Patient> items, string? phone)
    {
        if(phone is null) return items;
        return items.Where(i => i.Phone.Contains(phone));
    }
    public static IQueryable<Patient> WithEmail(this IQueryable<Patient> items, string? email)
    {
        if(email is null) return items;
        return items.Where(i => i.Email.Contains(email));
    }
    public static IQueryable<Patient> WithExactName(this IQueryable<Patient> items, string firstName, string lastName)
    {
        return items.Where(i => i.FirstName.ToLower().Equals(firstName.ToLower())
        && i.LastName.ToLower().Equals(lastName.ToLower()));
    }
    public static IQueryable<Patient> NotDeleted(this IQueryable<Patient> items)
    {
        return items.Where(i => !i.Deleted.HasValue);
    }
    public static IQueryable<Patient> Deleted(this IQueryable<Patient> items)
    {
        return items.Where(i => i.Deleted.HasValue);
    }
    public static IQueryable<Patient> OrderByCreateDate(this IQueryable<Patient> items, bool descending = false)
    {
        return descending ? items.OrderByDescending(i => i.Created) : (IQueryable<Patient>)items.OrderBy(i => i.Created);
    }
    public static IQueryable<Patient> OrderByName(this IQueryable<Patient> items, bool descending = false)
    {
        return descending ? items.OrderByDescending(i => i.FirstName).ThenByDescending(i => i.LastName) : items.OrderBy(i => i.FirstName).ThenBy(i => i.LastName);
    }
    //
}