using Appointment.Domain.DTO;
using Appointment.Domain.DTO.Availability;
using Appointment.Domain.DTO.Patient;
using Appointment.Domain.DTO.Visit;
using Appointment.Domain.Entities;
using Appointment.Domain.Entities.Identity;
using Appointment.Tests.Unit.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Tests.Unit.ObjectMothers;

public class EntityFactory
{
    public static class UserFactory
    {
        public static User CreateUser(string? login, string? email, string? firstName, string? lastName, string? phone, string? role = "user", DbSet<User>? users = null, int? id = null)
        {
            users ??= EmptyUsers;
            var user = new User(
                new AddUserDto()
                {
                    Login = login,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Phone = phone,
                    Role = role,
                },
            
            users);
            if (id is { }) user.Id = id.Value;
            return user;
        }
        private static readonly DbSet<User> EmptyUsers = DbSetMock.CreateDbSetMock(new List<User>()).Object;

    }
    public static class RoomFactory
    {
        public static Room CreateRoom(string? name, DbSet<Room>? rooms = null, int? id = null)
        {
            var room = new Room(name!, rooms ?? EmptyRooms);
            if (id.HasValue) room.Id = id.Value;
            return room;
        }
        private static readonly DbSet<Room> EmptyRooms = DbSetMock.CreateDbSetMock(new List<Room>()).Object;
    }
    public static class PatientFactory
    {
        public static Patient CreatePatient(string? firstName, string? lastName, string? phone, DbSet<Patient>? patients = null, int? id =null)
        {
            var patient = new Patient(
                new AddPatientDto()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Phone = phone,
                },
            patients ?? EmptyPatients);
            if (id is not null) patient.Id = id.Value;
            return patient;
        }
        private static readonly DbSet<Patient> EmptyPatients = DbSetMock.CreateDbSetMock(new List<Patient>()).Object;
    }
    public static class AvailabilityFactory
    {
        public static Availabillity CreateAvailability(int userId, DayOfWeek dayOfWeek, DateOnly activeFrom, DateOnly activeTo, TimeOnly start, TimeOnly end,
            DbSet<User>? users = null, DbSet<Availabillity>? availabillities = null, int? id = null)
        {
            var availability = new Availabillity(new AddAvailabilityDto()
            {
                UserId =userId,
                DayOfWeek = dayOfWeek,
                ActiveFrom = activeFrom,
                ActiveTo = activeTo,
                Start = start,
                End = end,
            },
            users ?? EmptyUsers,
            availabillities ?? EmptyAvailabilities);
            if (id is { }) availability.Id = id.Value;
            return availability;
        }
        private static readonly DbSet<Availabillity> EmptyAvailabilities = DbSetMock.CreateDbSetMock(new List<Availabillity>()).Object;
        private static readonly DbSet<User> EmptyUsers = DbSetMock.CreateDbSetMock(new List<User>()).Object;
    }
    public static class VisitFactory
    {
        public static Visit CreateVisit(int userId, int patientId, int roomId, DateTime date,TimeSpan duration, bool isRecuring,
            DbSet<User>? users = null, DbSet<Patient>? patients = null, DbSet<Room>? rooms = null, DbSet<Availabillity>? availabillities = null, DbSet<Visit>? visits = null, int? id = null)
        {
            var visit = new Visit(new AddVisitDto()
            {
                PatientId = patientId,
                TherapistId = userId,
                RoomId = roomId,
                DateStart = date,
                DateEnd = date.Add(duration),
                IsRecurring = isRecuring,
            },
            visits ?? EmptyVisits,
            users ?? EmptyUsers,
            patients ?? EmptyPatients,
            rooms ?? EmptyRooms,
            availabillities ?? EmptyAvailabilities
            );
            if (id is { }) visit.Id = id.Value;
            return visit;
        }
        private static readonly DbSet<Visit> EmptyVisits = DbSetMock.CreateDbSetMock(new List<Visit>()).Object;
        private static readonly DbSet<Room> EmptyRooms = DbSetMock.CreateDbSetMock(new List<Room>()).Object;
        private static readonly DbSet<User> EmptyUsers = DbSetMock.CreateDbSetMock(new List<User>()).Object;
        private static readonly DbSet<Patient> EmptyPatients = DbSetMock.CreateDbSetMock(new List<Patient>()).Object;
        private static readonly DbSet<Availabillity> EmptyAvailabilities = DbSetMock.CreateDbSetMock(new List<Availabillity>()).Object;
    }

}
