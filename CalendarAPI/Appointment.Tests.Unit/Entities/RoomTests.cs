using Appointment.Domain.DTO.Room;
using Appointment.Domain.Entities;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.Room;
using Appointment.Tests.Unit.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;
using static Appointment.Tests.Unit.ObjectMothers.EntityFactory;

namespace Appointment.Tests.Unit.Entities
{
    [TestFixture]
    internal class RoomTests
    {
        [Test]
        public void ShouldBeCreatedCorrectly()
        {
            // assign & act
            Room room = RoomFactory.CreateRoom("room 1");
            Assert.Multiple(() =>
            {
                // assert
                Assert.That(room.Name, Is.EqualTo("room 1"));
                Assert.That(room.Created, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
                Assert.That(room.Deleted, Is.Null);
            });
        }
        [Test]
        public void ShouldBeCreatedCorrectly_WithLongName()
        {
            var room = RoomFactory.CreateRoom(StringUtils.string128chars);
            Assert.Multiple(() =>
            {
                Assert.That(room.Name, Is.EqualTo(StringUtils.string128chars));
                Assert.That(room.Created, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
                Assert.That(room.Deleted, Is.Null);
            });
        }

        [Test]
        public void ShouldFailOnNameEmpty()
        {
            Assert.Throws<EmptyNameException>(delegate
            {
                var room = RoomFactory.CreateRoom("  ");
            });
        }
        [Test]
        public void ShouldFailOnNameTooLong()
        {
            Assert.Throws<RoomNameTooLongException>(delegate
            {
                var room = RoomFactory.CreateRoom(StringUtils.string129chars);
            });
        }
        [Test]
        public void SholudTrimName() {
            var room = RoomFactory.CreateRoom("          room 1              ");
            Assert.That(room.Name, Is.EqualTo("room 1"));
        }
        [Test]
        public void ShoulFailOnNameExisting() {
            var room = RoomFactory.CreateRoom("room1");
            var rooms = DbSetMock.CreateDbSetMock(new List<Room>() { room}).Object;
            Assert.Throws<RoomNameAlreadyExistsException>(delegate
            {
                RoomFactory.CreateRoom("room1",rooms);
            });
        }
    }
    [TestFixture]
    public class RoomUpdateTests
    {
        private Room _room;
        [SetUp]
        public void Setup()
        {
            _room = RoomFactory.CreateRoom("room 1"); _room.Id = 1;
        }
        [Test]
        public void ShouldBeUpdatedCorrectly()
        {
            _room.Update(new UpdateRoomDto() { Name = "room 2"});
            Assert.Multiple(() =>
            {
                Assert.That(_room.Name, Is.EqualTo("room 2"));
                Assert.That(_room.LastModified, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            });
        }
        [Test]
        public void ShouldBeUpdatedCorrectly_WithLongName()
        {
            _room.Update(new UpdateRoomDto() { Name = StringUtils.string128chars });
            Assert.Multiple(() =>
            {
                Assert.That(_room.Name, Is.EqualTo(StringUtils.string128chars));
                Assert.That(_room.LastModified, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            });
        }
        [Test]
        public void ShouldFailOnNameEmpty()
        {
            Assert.Throws<EmptyNameException>(delegate
            {
                _room.Update(new UpdateRoomDto() { Name = "  " });
            });
        }
        [Test]
        public void ShouldFailOnNameTooLong()
        {
            Assert.Throws<RoomNameTooLongException>(delegate
            {
                _room.Update(new UpdateRoomDto() { Name = StringUtils.string129chars });
            });
        }
        [Test]
        public void SholudTrimName()
        {
            _room.Update(new UpdateRoomDto() { Name = "          room 2              " });
            Assert.That(_room.Name, Is.EqualTo("room 2"));
        }
        [Test]
        public void ShouldFailOnNameInUse()
        {
            var room2 = RoomFactory.CreateRoom("room 2"); room2.Id = 2;
            var rooms = DbSetMock.CreateDbSetMock(new List<Room>() { _room, room2 }).Object;
            Assert.Throws<RoomNameAlreadyExistsException>(delegate
            {
                _room.Update(new UpdateRoomDto() { Name = "room 2" }, rooms);
            });
        }
    }
    [TestFixture]
    public class RoomDeleteTests
    {
        private User _user;
        private DbSet<User> _users;

        private Room _room;
        private DbSet<Room> _rooms;

        private Patient _patient;
        private DbSet<Patient> _patients;

        private Availabillity _availabillity;
        private DbSet<Availabillity> _availabillities;

        private Visit _visit;
        [SetUp]
        public void Setup()
        {
            _user = UserFactory.CreateUser("akowalski", "andrzej.kowalski@gmail.com", "Andrzej", "Kowalski", "000111222"); _user.Id = 1;
            _users = DbSetMock.CreateDbSetMock(new[] { _user }).Object;

            _room = RoomFactory.CreateRoom("room 1"); _room.Id = 1;
            _rooms = DbSetMock.CreateDbSetMock(new[] { _room }).Object;

            _patient = PatientFactory.CreatePatient("Jan", "Nowak", "999333222"); _patient.Id = 1;
            _patients = DbSetMock.CreateDbSetMock(new[] { _patient }).Object;

            _availabillity = AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Wednesday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0), new TimeOnly(16, 0), _users);
            _availabillities = DbSetMock.CreateDbSetMock(new[] { _availabillity }).Object;

            _visit = VisitFactory.CreateVisit(1, 1, 1, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities);
        }
        [Test]
        public void ShouldBeDeletedCorrectly()
        {
            _room.MarkAsDeleted();
            Assert.That(_room.Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
        [Test]
        public void ShouldBeDeletedCorrectly_WithVisits()
        {
            _room.Visits = new() { _visit };
            _room.MarkAsDeleted();
            Assert.Multiple(() =>
            {
                Assert.That(_room.Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
                Assert.That(_room.Visits.First().Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            });
        }
    }
    [TestFixture]
    public class RoomSpecificationTests
    {
        private List<Room> _roomList;
        private IQueryable<Room> _rooms;
        [SetUp]
        public void Setup()
        {
            _roomList = new List<Room>()
            {
                RoomFactory.CreateRoom("conference"),
                RoomFactory.CreateRoom("room 2"),
                RoomFactory.CreateRoom("room 3"),
                RoomFactory.CreateRoom("room 4"),
            };
            _rooms = _roomList.AsQueryable();
        }
        [Test]
        public void ShouldPaginateCorrectly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_rooms.Page(2, 2), Is.EquivalentTo(_rooms.Skip(2).Take(2)));
                Assert.That(_rooms.Page(1, 2), Is.EquivalentTo(_rooms.Take(2)));
                Assert.That(_rooms.Page(3, 2), Is.EquivalentTo(_rooms.Skip(4)));
            });
        }
        [TestCase(0,1)]
        [TestCase(0,-1)]
        [TestCase(1,0)]
        [TestCase(1,-20)]
        public void ShouldFailOnInvalidPagination(int page, int pageSize)
        {
            Assert.Throws<InvalidPaginationException>(delegate
            {
                _rooms.Page(page, pageSize);
            });
        }
        [Test]
        public void ShouldFilterNotDeletedCorrectly()
        {
            var indexToDelete = 1;
            _roomList[indexToDelete].MarkAsDeleted();
            Assert.That(_rooms.NotDeleted(), Is.EquivalentTo(_rooms.Except(new[] { _roomList[indexToDelete] })));
        }
        [Test]
        public void ShouldFilterDeletedCorrectly()
        {
            var indexToDelete = 1;
            _roomList[indexToDelete].MarkAsDeleted();
            Assert.That(_rooms.Deleted(), Is.EquivalentTo(new[] { _roomList[indexToDelete] }));
        }
        [Test]
        public void ShouldOrderByCreateDateCorrectly()
        {
            Assert.That(_rooms.OrderByCreateDate(), Is.EqualTo(_roomList).AsCollection);
        }
        [Test]
        public void ShouldOrderByNameCorrectly()
        {
            Assert.That(_rooms.OrderByName(), Is.EqualTo(_roomList).AsCollection);
        }
        [TestCase("conference")]
        [TestCase("confe")]
        [TestCase("Confe")]
        [TestCase("CONFE ")]
        [TestCase("   CONFE    ")]
        public void ShouldFilterByNameCorrectly(string name)
        {
            Assert.That(_rooms.WithName(name), Is.EquivalentTo(new[] { _roomList[0] }));
        }
    }
}
