using Appointment.Common.Utils;
using Appointment.Domain.DTO.Room;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.Room;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Entities;

public class Room : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Visit> Visits { get; set; } = new();

    public Room(string? name, DbSet<Room> rooms) : base()
    {
        Name = name?.Trim();
        Visits = new();
        Validate(rooms);
    }
    public Room(AddRoomDto dto, DbSet<Room> rooms) : this(dto.Name, rooms) { }
    public void Update(UpdateRoomDto dto, DbSet<Room>? rooms = null)
    {
        if(dto.Name.NeedsUpdate)
        {
            Name = dto.Name.NewValue.Trim();
        }
        Validate(rooms);
        LastModified = DateTime.UtcNow;
    }

    public Room() { }
    private void Validate(DbSet<Room>? rooms = null)
    {
        if(rooms is not null) Require.IsEmpty(rooms.NotDeleted().WithExactName(Name)).OrError(new RoomNameAlreadyExistsException(Name));
        Require.NotEmpty(Name).OrError(new EmptyNameException());
        Require.IsFalse(Name.Length > 128).OrError(new RoomNameTooLongException(Name));
    }
    public GetRoomDto Present()
    {
        return new GetRoomDto(Id, Name);
    }
    public void MarkAsDeleted()
    {
        Deleted = DateTime.UtcNow;
        foreach(var visit in Visits)
        {
            visit.MarkAsDeleted();
        }
    }
}

public static class RoomSpecification
{
    public const int defaultPage = 1;
    public const int defaultPageSize = 10;
    public static IQueryable<Room> Page(this IQueryable<Room> items, int? page, int? pageSize)
    {
        page ??= defaultPage;
        pageSize ??= defaultPageSize;

        if(pageSize.Value <= 0 || page.Value <= 0)
        {
            throw new InvalidPaginationException(page, pageSize);
        }
        return items.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
    }
    public static async Task<List<GetRoomDto>> PresentedOnListAsync(this IQueryable<Room> items)
    {
        return await items.Select(i => i.Present()).ToListAsync();
    }
    public static IQueryable<Room> NotDeleted(this IQueryable<Room> items)
    {
        return items.Where(i => !i.Deleted.HasValue);
    }
    public static IQueryable<Room> Deleted(this IQueryable<Room> items)
    {
        return items.Where(i => i.Deleted.HasValue);
    }
    public static IQueryable<Room> WithName(this IQueryable<Room> items, string? name)
    {
        if(string.IsNullOrEmpty(name))
        {
            return items;
        }
        return items.Where(i => i.Name.ToLower().Contains(name.Trim().ToLower()));
    }
    public static IQueryable<Room> WithExactName(this IQueryable<Room> items, string name)
    {
        return items.Where(i => i.Name == name);
    }
    public static Room? WithId(this IQueryable<Room> items, int id)
    {
        return items.SingleOrDefault(i => i.Id == id);
    }
    public static async Task<Room?> WithIdAsync(this IQueryable<Room> items, int id)
    {
        return await items.SingleOrDefaultAsync(i => i.Id == id);
    }
    public static IQueryable<Room> OrderByCreateDate(this IQueryable<Room> items, bool descending = false)
    {
        return descending ? items.OrderByDescending(i => i.Created) : items.OrderBy(i => i.Created);
    }
    public static IQueryable<Room> OrderByName(this IQueryable<Room> items, bool descending = false)
    {
        return descending ? items.OrderByDescending(i => i.Name) : items.OrderBy(i => i.Name);
    }
}