using Appointment.Common.Utils;
using Appointment.Domain.DTO.Visit;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.Visit;
using Microsoft.EntityFrameworkCore;

namespace Appointment.Domain.Entities;

public class Visit : AuditableEntity
{

    public int Id { get; set; }
    public User Therapist { get; set; }
    public int TherapistId { get; set; }
    public Patient Patient { get; set; }
    public int PatientId { get; set; }
    public Room Room { get; set; }
    public int RoomId { get; set; }
    public DateTime Date { get; set; }  
    public DateTime DateTo { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsFirstVisit { get; set; }
    // TODO: do zrobienia opcja powtarzania wizyt
    public bool IsRecurring { get; set; }
    public Visit(AddVisitDto dto, DbSet<Visit> visits, DbSet<User> users,DbSet<Patient> patients,DbSet<Room> rooms, DbSet<Availabillity> availabillities)
    {
        PatientId = dto.PatientId;
        TherapistId = dto.TherapistId;
        RoomId = dto.RoomId;
        Date = dto.DateStart;
        DateTo = dto.DateEnd;
        Duration = DateTo - Date;
        IsRecurring = dto.IsRecurring;
        Validate(visits, users,patients,rooms, availabillities);
    }
    public void Update(UpdateVisitDto dto, DbSet<Visit> visits, DbSet<User> users, DbSet<Patient> patients, DbSet<Room> rooms, DbSet<Availabillity> availabillities)
    {
        if(dto.TherapistId.NeedsUpdate) TherapistId = dto.TherapistId.NewValue;
        if(dto.PatientId.NeedsUpdate) PatientId = dto.PatientId.NewValue;
        if(dto.RoomId.NeedsUpdate) RoomId = dto.RoomId.NewValue;
        if(dto.DateStart.NeedsUpdate) Date = dto.DateStart.NewValue;
        if(dto.DateEnd.NeedsUpdate) DateTo = dto.DateEnd.NewValue;

        Validate(visits, users, patients, rooms, availabillities);
        LastModified = DateTime.UtcNow;
    }
    public Visit() { }
    public void MarkAsDeleted()
    {
        Deleted = DateTime.UtcNow;
    }
    public GetVisitDto Present()
    {
        return new(Id, TherapistId, PatientId, RoomId, Date, Duration, IsFirstVisit, IsRecurring);
    }
    public void Validate(DbSet<Visit> visits, DbSet<User> users,DbSet<Patient> patients, DbSet<Room> rooms, DbSet<Availabillity> availabillities)
    {
        Require.IsTrue(Duration >= TimeSpan.Zero).OrError(new DateEndBeforeStartException(Date,DateTo));
        if(users?.NotDeleted().WithId(TherapistId) is null) throw new NoVisitUserException(TherapistId);
        if(patients?.NotDeleted().WithId(PatientId) is null) throw new NoVisitpatientException(PatientId);
        if(rooms?.NotDeleted().WithId(RoomId) is null) throw new NoVisitRoomException(RoomId);
        if(availabillities is { }) Require.IsTrue(availabillities.NotDeleted().FromDay(Date).FromTime(TimeOnly.FromDateTime(Date), Duration, true).Any()).OrError(new NoAvailabilityInDateException(TherapistId, Date, Date.Add(Duration)));

        if(visits is { })
        {
            var withDateDiffrentId = visits.NotDeleted().WithDate(Date, DateTo).Where(v => v.Id != Id);
            Require.IsEmpty(withDateDiffrentId.WithRoom(RoomId)).OrError(new RoomIsInUseThisDateException(RoomId, Date, Date.Add(Duration)));
            Require.IsEmpty(withDateDiffrentId.WithUser(TherapistId)).OrError(new UserHasVisitInThisDateException(TherapistId, Date, Date.Add(Duration)));
            Require.IsEmpty(withDateDiffrentId.WithPatient(PatientId)).OrError(new PatienthasVisitInthisDateException(PatientId, Date, Date.Add(Duration)));
        }
    }
}

public static class VisitSpecification
{
    public const int defaultPage = 1;
    public const int defaultPageSize = 10;

    public static IQueryable<Visit> Page(this IQueryable<Visit> items, int? page, int? pageSize)
    {
        page ??= defaultPage;
        pageSize ??= defaultPageSize;

        if(pageSize.Value <= 0 || page.Value <= 0)
        {
            throw new InvalidPaginationException(page, pageSize);
        }
        return items.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
    }

    public static async Task<List<GetVisitDto>> PresentedOnListAsync(this IQueryable<Visit> items)
    {
        return await items.Select(item => item.Present()).ToListAsync();
    }
    public static IQueryable<Visit> WithUser(this IQueryable<Visit> items, int? userId)
    {
        if(userId.HasValue)
            return items.Where(i => i.TherapistId == userId);
        return items;
    }
    public static IQueryable<Visit> WithPatient(this IQueryable<Visit> items, int? patientId)
    {
        if(patientId.HasValue)
            return items.Where(i => i.PatientId == patientId);
        return items;
    }
    public static IQueryable<Visit> WithRoom(this IQueryable<Visit> items, int? roomId)
    {
        if (roomId.HasValue)
            return items.Where(i => i.RoomId == roomId);
        return items;
    }
    public static IQueryable<Visit> NotDeleted(this IQueryable<Visit> items)
    {
        return items.Where(i => !i.Deleted.HasValue);
    }
    public static IQueryable<Visit> Deleted(this IQueryable<Visit> items)
    {
        return items.Where(i => i.Deleted.HasValue);
    }

    public static Visit WithId(this IQueryable<Visit> items, int id)
    {
        return items.SingleOrDefault(i => i.Id == id);
    }
    public static async Task<Visit> WithIdAsync(this IQueryable<Visit> items, int id)
    {
        return await items.SingleOrDefaultAsync(i => i.Id == id);
    }
    public static IQueryable<Visit> WithDate(this IQueryable<Visit> items, DateTime? dateFrom, DateTime? dateTo, bool inclusive = false)
    {
        dateFrom ??= DateTime.MinValue;
        dateTo ??= DateTime.MaxValue;
        return inclusive
            ? items.Where(i => i.Date <= dateTo && dateFrom <= i.DateTo)
            : items.Where(i => i.Date < dateTo && dateFrom < i.DateTo);
    }
    public static IQueryable<Visit> OrderByCreate(this IQueryable<Visit> items, bool dsc = false)
    {
        return dsc ? items.OrderByDescending(i => i.Created) : items.OrderBy(i => i.Created);
    }
    public static IQueryable<Visit> OrderByDate(this IQueryable<Visit> items, bool dsc = false)
    {
        return dsc ? items.OrderByDescending(i => i.Date) : items.OrderBy(i => i.Date);
    }
    
}
