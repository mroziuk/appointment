using Appointment.Common.Utils;
using Appointment.Domain.DTO;
using Appointment.Domain.DTO.User;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.Identity;
using Appointment.Domain.Exceptions.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Appointment.Domain.Entities;

public class User : AuditableEntity
{

    public int Id { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Phone { get; set; }
    public string PasswordHash { get; set; }
    public List<Visit> Visits { get; set; }
    public List<Availabillity> Availabillities { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public string Role { get; set; }
    public string? ResetToken { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; init; }
    public User(string login, string firstName, string lastName, string email, string passwordHash, string role, DbSet<User> users = null, bool isSuperAdmin = false) : base()
    {
        Login = login.Trim();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        PasswordHash = passwordHash;
        Role = !string.IsNullOrEmpty(role) ? role.Trim().ToLowerInvariant() : Identity.Role.User;
        Visits = new();
        Availabillities = new();
        RefreshTokens = new List<RefreshToken>();
        Validate(users);
    }
    public User(AddUserDto dto, DbSet<User> users) : base()
    {
        Login = dto.Login.Trim();
        Email = dto.Email.Trim();
        FirstName = dto.FirstName.Trim();
        LastName = dto.LastName.Trim();
        Phone = dto.Phone?.Trim();
        Role = dto.Role.Trim().ToLowerInvariant();
        PasswordHash = string.Empty;
        Visits = new();
        Availabillities = new();
        RefreshTokens = new List<RefreshToken>();
        Validate(users);
    }
    public void Update(UpdateUserDto dto, DbSet<User>? users = null)
    {
        if(Role.Equals(Identity.Role.SuperAdmin) && !dto.Role.Equals(Identity.Role.SuperAdmin))
        {
            throw new CannotDeleteSuperAdminException();
        }
        if(dto.Role.NeedsUpdate
            && Identity.Role.Admin != dto.Role
            && Role.Equals(Identity.Role.Admin)
            && users != null
            && users.NotDeleted().WithRole(Identity.Role.Admin).Any(u => u.Id != Id))
        {
            throw new CannotDeleteLastAdminException();
        }
        if(dto.Role.NeedsUpdate) Role = !string.IsNullOrEmpty(dto.Role.NewValue) ? dto.Role.NewValue.Trim().ToLowerInvariant() : dto.Role.NewValue;
        if(dto.Login.NeedsUpdate) Login = !string.IsNullOrEmpty(dto.Login.NewValue) ? dto.Login.NewValue.Trim() : dto.Login.NewValue;
        if(dto.FirstName.NeedsUpdate) FirstName = !string.IsNullOrEmpty(dto.FirstName.NewValue) ? dto.FirstName.NewValue.Trim() : dto.FirstName.NewValue;
        if(dto.LastName.NeedsUpdate) LastName = !string.IsNullOrEmpty(dto.LastName.NewValue) ? dto.LastName.NewValue.Trim() : dto.LastName.NewValue;
        if(dto.Email.NeedsUpdate) Email = !string.IsNullOrEmpty(dto.Email.NewValue) ? dto.Email.NewValue.Trim() : dto.Email.NewValue;
        if(dto.Phone.NeedsUpdate) Phone = !string.IsNullOrEmpty(dto.Phone.NewValue) ? dto.Phone.NewValue.Trim() : dto.Phone.NewValue;
        if(dto.IsActive.NeedsUpdate) IsActive = dto.IsActive.NewValue;

        Validate(users);
        LastModified = DateTime.UtcNow;
    }
    public User() { }
    public UserInfoDto Present()
    {
        var result = new UserInfoDto(
                Id,
                FirstName,
                LastName,
                Email,
                Role
            );
        return result;
    }

    private void Validate(DbSet<User> users)
    {
        Require.NotEmpty(Login).OrError(new EmptyLoginException());
        Require.IsFalse(Login.Length > 128).OrError(new LoginTooLongException(Login));
        Require.IsFalse(Login.Length < 4).OrError(new LoginTooShortException(Login));
        Require.NotEmpty(Email).OrError(new EmptyEmailException());
        Require.IsTrue(EmailUtils.IsValidEmail(Email)).OrError(new InvalidEmailException(Email));
        if (users is { })
        {
            Require.IsEmpty(users.WithEmail(Email)).OrError(new EmailAlreadyInUseException(Email));
            Require.IsEmpty(users.WithEmail(Login)).OrError(new LoginAlreadyExistException(Login));
        }
    }
    public bool Activate()
    {
        if (IsActive) return false;
        IsActive = true;
        ResetToken = null;
        return true;
    }
    public void MarkAsDeleted()
    {
        Deleted = DateTime.UtcNow;
        foreach (var visit in Visits)
        {
            visit.MarkAsDeleted();
        }
        foreach (var availability in Availabillities)
        {
            availability.MarkAsDeleted();
        }
    }
}


public static class UserSpecification
{
    public const int defaultPage = 1;
    public const int defaultPageSize = 10;

    public static async Task<List<UserInfoDto>> PresentedOnListAsync(this IQueryable<User> items)
    {
        return await items.Select(item => item.Present()).ToListAsync();
    }
    public static IQueryable<User> Page(this IQueryable<User> items, int? page, int? pageSize)
    {
        page ??= defaultPage;
        pageSize ??= defaultPageSize;

        if (pageSize <= 0 || page.Value <= 0)
        {
            throw new InvalidPaginationException(page, pageSize);
        }
        return items.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
    }
    public static User WithId(this IQueryable<User> items, int id)
    {
        return items.FirstOrDefault(i => i.Id == id);
    }
    public static async Task<User?> WithIdAsync(this IQueryable<User> items, int id)
    {
        return await items.SingleOrDefaultAsync(i => i.Id == id);
    }
    public static IQueryable<User> WithName(this IQueryable<User> items, string? name)
    {
        if (name is null) return items;
        name = name.Trim();
        return items.Where(i => i.FirstName.ToLower().Contains(name.ToLower()) || i.LastName.ToLower().Contains(name.ToLower()));
        //return items.Where(i => 
        //    i.LastName.ToLower().ContainsAny(name.Split()) ||
        //    i.FirstName.ToLower().ContainsAny(name.Split()) ||
        //    name.Split().Any(x => x.Contains(i.FirstName, StringComparison.OrdinalIgnoreCase)) ||
        //    name.Split().Any(x => x.Contains(i.FirstName, StringComparison.OrdinalIgnoreCase)));
    }
    public static IQueryable<User> WithLogin(this IQueryable<User> items, string? login)
    {
        if (login is null) return items;
        return items.Where(i => i.Login.Contains(login.ToLower()));
    }
    public static IQueryable<User> WithEmail(this IQueryable<User> items, string? email)
    {
        if (email is null) return items;
        return items.Where(i => i.Email.Contains(email!.ToLower()));
    }
    public static IQueryable<User> NotDeleted(this IQueryable<User> items)
    {
        return items.Where(i => !i.Deleted.HasValue);
    }
    public static IQueryable<User> Deleted(this IQueryable<User> items)
    {
        return items.Where(i => i.Deleted.HasValue);
    }
    public static IQueryable<User> WithRole(this IQueryable<User> items, string? role)
    {
        if (role is null) return items;
        return items.Where(i => i.Role.Equals(role.ToLower()));
    }
    public static IQueryable<User> Active(this IQueryable<User> items)
    {
        return items.Where(i => i.IsActive);
    }
    public static IQueryable<User> OrderByCreateDate(this IQueryable<User> items, bool descending = false)
    {
        return descending ? items.OrderByDescending(i => i.Created) : items.OrderBy(i => i.Created);
    }
    public static IQueryable<User> OrderByName(this IQueryable<User> items, bool descending = false)
    {
        return descending ? items.OrderByDescending(i => i.LastName).ThenByDescending(i => i.FirstName) : items.OrderBy(i => i.LastName).ThenBy(i => i.FirstName);
    }

    public static bool ContainsAny(this string haystack, params string[] needles)
    {
        foreach (string needle in needles)
        {
            if (haystack.Contains(needle, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
    private static readonly Regex sWhitespace = new Regex(@"\s+");
}