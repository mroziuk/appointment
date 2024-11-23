using Appointment.Common.Utils;
using Appointment.Domain.DTO.Availability;
using Appointment.Domain.Entities.Identity;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.Availability;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Appointment.Domain.Entities;

public class Availabillity : AuditableEntity
{
    public int Id{ get; set; }
    public User? User { get; set; }
    public  int UserId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public DateOnly ActiveFrom { get; set; }
    public DateOnly ActiveTo { get; set; }
    // TODO: dodać opcję zrobienia tego powtarzalnie, dodać początek i koniec działania tego, wymyślić jak obsługiwać dni wolne, urlopy i czy nie przerobić tego z TimeOnly na DateTime
    public Availabillity() { }
    public Availabillity(AddAvailabilityDto dto,DbSet<User> users, DbSet<Availabillity> availabillities)
    {
        UserId = dto.UserId;
        DayOfWeek = dto.DayOfWeek;
        Start = dto.Start;
        End = dto.End;
        ActiveFrom = dto.ActiveFrom;
        ActiveTo = dto.ActiveTo;
        Validate(users,availabillities);
    }
    public void Update(UpdateAvailabilityDto dto, DbSet<User>? users, DbSet<Availabillity>? availabillities)
    {
        if(dto.UserId.NeedsUpdate) UserId = dto.UserId.NewValue;
        if(dto.DayOfWeek.NeedsUpdate) DayOfWeek = (DayOfWeek)dto.DayOfWeek.NewValue;
        if(dto.Start.NeedsUpdate) Start = dto.Start.NewValue;
        if(dto.End.NeedsUpdate) End = dto.End.NewValue;
        if(dto.ActiveFrom.NeedsUpdate) ActiveFrom = dto.ActiveFrom.NewValue;
        if(dto.ActiveTo.NeedsUpdate) ActiveTo = dto.ActiveTo.NewValue;

        Validate(users, availabillities);
        LastModified = DateTime.UtcNow;
    }
    public void MarkAsDeleted()
    {
        Deleted = DateTime.UtcNow;
    }
    private void Validate(DbSet<User>? users = null, DbSet<Availabillity>? availabillities = null)
    {
        if(users is not null && !users.NotDeleted().Any(u => u.Id == UserId))
        {
            throw new NoAvailabilityUserException(UserId);
        }
        var overlaping = availabillities?.NotDeleted().Overlaping(this).FirstOrDefault();
        if(overlaping is not null)
        {
            throw new AvailabilityOverlapException(overlaping.Id, overlaping.Start, overlaping.End);
        }
        // this will be used to check from update
        Require.NotEmpty(Start).OrError(new NoAvailabilityStartException());
        Require.NotEmpty(End).OrError(new NoAvailabilityEndException());
        Require.NotEmpty(ActiveFrom).OrError(new NoAvailabilityActiveFromDateException());
        Require.NotEmpty(ActiveTo).OrError(new NoAvailabilityActiveToDateException());
        // start before end
        Require.IsTrue(Start < End).OrError(new TimeOnlyEndBeforeStartException(Start, End));
        Require.IsTrue(ActiveFrom <= ActiveTo).OrError(new DateOnlyEndBeforeStartException(ActiveFrom, ActiveTo)); // Availability can be for one day only
    }
    public GetAvailabilityDto Present() =>  new()
    {
        Id = Id,
        DayOfWeek = (int)DayOfWeek,
        End = End,
        Start = Start,
        ActiveFrom = ActiveFrom.ToDateTime(TimeOnly.MinValue),
        ActiveTo = ActiveTo.ToDateTime(TimeOnly.MinValue),
        UserId = UserId,
    };
}

public static class AvailabillitySpecification
{
    public const int defaultPage = 1;
    public const int defaultPageSize = 10;
    public static IQueryable<Availabillity> PresentedOnList(this IQueryable<Availabillity> items)
    {
        return items;
    }
    public static IQueryable<Availabillity> WithUser(this IQueryable<Availabillity> items, int? id)
    {
        if(id.HasValue)
            return items.Where(i => i.UserId == id).AsQueryable();
        return items;
    }
    public static IQueryable<Availabillity> NotDeleted(this IQueryable<Availabillity> items)
    {
        return items.Where(i => !i.Deleted.HasValue);
    }
    public static IQueryable<Availabillity> FromDayOfWeek(this IQueryable<Availabillity> items, int? dayOfWeek)
    {
        return dayOfWeek.HasValue
            ? items.Where(i => i.DayOfWeek == (DayOfWeek)dayOfWeek)
            : items;
    }
    public static IQueryable<Availabillity> FromDate(this IQueryable<Availabillity> items, DateOnly? fromDate)
    {
        fromDate ??= DateOnly.MinValue;
        return items.Where(i => i.ActiveFrom >= fromDate);
    }
    public static IQueryable<Availabillity> FromDay(this IQueryable<Availabillity> items, DateTime date)
    {
        return items.Where(i =>
            i.DayOfWeek == date.DayOfWeek &&
            i.ActiveFrom <= DateOnly.FromDateTime(date) &&
            i.ActiveTo >= DateOnly.FromDateTime(date));
    }
    public static IQueryable<Availabillity> ToDate(this IQueryable<Availabillity> items, DateOnly? toDate)
    {
        toDate ??= DateOnly.MaxValue;
        return items.Where(i => i.ActiveTo <= toDate);
    }
    public static IQueryable<Availabillity> Page(this IQueryable<Availabillity> items, int? page, int? pageSize)
    {
        page ??= defaultPage;
        pageSize ??= defaultPageSize;
        if(pageSize.Value <= 0 || page.Value <= 0)
        {
            throw new InvalidPaginationException(page, pageSize);
        }
        return items.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
    }
    public static async Task<List<GetAvailabilityDto>> PresentedOnListAsync(this IQueryable<Availabillity> items)
    {
        return await items.Select(i => i.Present()).ToListAsync();
    }
    public static IQueryable<Availabillity> FromWeek(this IQueryable<Availabillity> items, DateOnly startDate)
    {
        return items.Where(i => i.ActiveFrom <= startDate.AddDays(7) || i.ActiveTo >= startDate);
    }
    public static GetAvailabilityDto? WithId(this IQueryable<Availabillity> items, int id)
    {
        return items.SingleOrDefault(i => i.Id == id)?.Present();
    }
    public static async Task<Availabillity> WithIdAsync(this IQueryable<Availabillity> items, int id)
    {
        return await items.FirstOrDefaultAsync(i => i.Id == id);
    }
    public static IQueryable<Availabillity> FromTime(this IQueryable<Availabillity> items, TimeOnly start, TimeSpan duration, bool inclusive = false)
    {
        return inclusive
            ? items.Where(i => i.Start <= start.Add(duration) && start <= i.End)
            : items.Where(i => i.Start < start.Add(duration) && start < i.End && i.Start != start);
    }
    public static IQueryable<Availabillity> Overlaping(this IQueryable<Availabillity> items, Availabillity availability)
    {
        return items
            .Where(
                i => i.Id != availability.Id
                && i.UserId == availability.UserId
                && i.DayOfWeek == availability.DayOfWeek
                && i.Start <= availability.End && availability.Start <= i.End
                && i.ActiveFrom <= availability.ActiveTo && availability.ActiveFrom <= i.ActiveTo
            );
    }
}