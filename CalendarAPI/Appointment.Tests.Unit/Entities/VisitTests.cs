using Appointment.Domain.Entities;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.Visit;
using Appointment.Tests.Unit.Utils;
using Microsoft.EntityFrameworkCore;
using static Appointment.Tests.Unit.ObjectMothers.EntityFactory;

namespace Appointment.Tests.Unit.Entities
{
    [TestFixture]
    public class VisitTests
    {
        private User _user, _user2;
        private Room _room, _room2;
        private Patient _patient, _patient2;
        private Availabillity _availabillity;

        private DbSet<User> _users;
        private DbSet<Room> _rooms;
        private DbSet<Patient> _patients;
        private DbSet<Availabillity> _availabillities;

        [SetUp]
        public void SetUp()
        {
            _user = UserFactory.CreateUser("akowalski", "andrzej.kowalski@gmail.com", "Andrzej", "Kowalski", "000111222","user",null,1);
            _user2 = UserFactory.CreateUser("rsikora", "roman.sikora@o2.pl", "Roman", "Sikora", "333222111","user", null,2);
            _users = DbSetMock.CreateDbSetMock(new[] { _user,_user2 }).Object;

            _room = RoomFactory.CreateRoom("room 1",null,1);
            _room2 = RoomFactory.CreateRoom("room 2",null,2);
            _rooms = DbSetMock.CreateDbSetMock(new[] { _room,_room2 }).Object;

            _patient = PatientFactory.CreatePatient("Jan", "Nowak", "999333222",null,1);
            _patient2 = PatientFactory.CreatePatient("Daniel", "Obajtek", "999444222",null,2);
            _patients = DbSetMock.CreateDbSetMock(new[] { _patient,_patient2 }).Object;

            _availabillity = AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Wednesday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0), new TimeOnly(16, 0),_users,null,1);
            _availabillities = DbSetMock.CreateDbSetMock(new[] { _availabillity }).Object;
        }

        [Test]
        public void ShoulBeCreatedCorrectly()
        {
            // arrange & act
            var visit = VisitFactory.CreateVisit(_user.Id,_patient.Id,_room.Id,new DateTime(2023,2,1,9,0,0),new TimeSpan(1,0,0),false,_users,_patients,_rooms,_availabillities);
            // assert
            Assert.That(visit.PatientId, Is.EqualTo(_patient.Id));
            Assert.That(visit.TherapistId, Is.EqualTo(_user.Id));
            Assert.That(visit.RoomId, Is.EqualTo(_room.Id));
            Assert.That(visit.Deleted, Is.Null);

            Assert.That(visit.Date, Is.EqualTo(new DateTime(2023, 2, 1, 9, 0, 0)));
            Assert.That(visit.DateTo, Is.EqualTo(new DateTime(2023, 2, 1, 10, 0, 0)));
            Assert.That(visit.Duration, Is.EqualTo(new TimeSpan(1, 0, 0)));
        }
        [Test]
        public void ShouldFailOnPatientNotExist()
        {
            Assert.Throws<NoVisitpatientException> (() => VisitFactory.CreateVisit(_user.Id, -1, _room.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities));
        }
        [Test]
        public void ShouldFailOnTherapistNotExist()
        {
            Assert.Throws<NoVisitUserException>(() => VisitFactory.CreateVisit(-1, _patient.Id, _room.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities));
        }
        [Test]
        public void ShouldFailOnAvailabilityNotExist()
        {
            Assert.Throws<NoAvailabilityInDateException>(() => VisitFactory.CreateVisit(_user.Id, _patient.Id, _room.Id, new DateTime(2023, 3, 2, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities));
        }
        [Test]
        public void ShouldFailOnRoomNotExist()
        {
            Assert.Throws<NoVisitRoomException>(() => VisitFactory.CreateVisit(_user.Id, _patient.Id, -1, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities));
        }
        [Test]
        public void ShouldFailOnPatientHasColidingVisits()
        {
            var visit = VisitFactory.CreateVisit(_user.Id, _patient.Id, _room.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities,null,1);
            var visits = DbSetMock.CreateDbSetMock(new[] { visit }).Object;

            Assert.Throws<PatienthasVisitInthisDateException>(() => VisitFactory.CreateVisit(_user2.Id, _patient.Id, _room2.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities,visits));
        }
        [Test]
        public void ShouldFailOnRoomIsNotAvailable()
        {
            var visit = VisitFactory.CreateVisit(_user.Id, _patient.Id, _room.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities,null,1);
            var visits = DbSetMock.CreateDbSetMock(new[] { visit }).Object;
Assert.Throws<RoomIsInUseThisDateException>(() => VisitFactory.CreateVisit(_user2.Id, _patient2.Id, _room.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities,visits));
        }
        [Test]
        public void ShouldFailOnUserHasCollidingVisits()
        {
            var visit = VisitFactory.CreateVisit(_user.Id, _patient.Id, _room.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities,null,1);
            var visits = DbSetMock.CreateDbSetMock(new[] { visit }).Object;
            Assert.Throws<UserHasVisitInThisDateException>(() => VisitFactory.CreateVisit(_user.Id, _patient2.Id, _room2.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities,visits));
        }
        [Test]
        public void ShouldFailOnDateToIsBeforeDateFrom()
        {
            Assert.Throws<DateEndBeforeStartException>(() => VisitFactory.CreateVisit(_user.Id, _patient.Id, _room.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(-1, 0, 0), false, _users, _patients, _rooms, _availabillities));
        }
        [Test]
        public void ShouldBrDeletedCorrectly()
        {
            var visit = VisitFactory.CreateVisit(_user.Id, _patient.Id, _room.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, _patients, _rooms, _availabillities);
            visit.MarkAsDeleted();
            Assert.That(visit.Deleted, Is.Not.Null);
            Assert.That(visit.Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
        [Test]
        public void ShouldPresentCorrectly()
        {
            var visit = VisitFactory.CreateVisit(_user.Id, _patient.Id, _room.Id, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), true, _users, _patients, _rooms, _availabillities);
            var dto = visit.Present();

            Assert.That(dto.PatientId, Is.EqualTo(_patient.Id));
            Assert.That(dto.TherapistId, Is.EqualTo(_user.Id));
            Assert.That(dto.RoomId, Is.EqualTo(_room.Id));
            Assert.That(dto.StartDate, Is.EqualTo(new DateTime(2023, 2, 1, 9, 0, 0)));
            Assert.That(dto.EndDate, Is.EqualTo(new DateTime(2023, 2, 1, 9, 0, 0).AddHours(1)));
            Assert.That(dto.IsFirstVisit, Is.False);
            Assert.That(dto.IsRecurring, Is.True);
        }
    }
    [TestFixture]
    public class VisitSpecificationTests
    {
        private User _user;
        private Room _room;
        private Patient _patient;
        private Availabillity _availabillity;

        private DbSet<User> _users;
        private DbSet<Room> _rooms;
        private DbSet<Patient> _patients;
        private DbSet<Availabillity> _availabillities;

        private List<Visit> _visitList;
        private IQueryable<Visit> _visits;
        [SetUp]
        public void SetUp()
        {
            _user = UserFactory.CreateUser("akowalski", "andrzej.kowalski@gmail.com", "Andrzej", "Kowalski", "000111222"); _user.Id = 1;
            _users = DbSetMock.CreateDbSetMock(new[] { _user }).Object;

            _room = RoomFactory.CreateRoom("room 1"); _room.Id = 1;
            _rooms = DbSetMock.CreateDbSetMock(new[] { _room }).Object;

            _patient = PatientFactory.CreatePatient("Jan", "Nowak", "999333222"); _patient.Id = 1;
            _patients = DbSetMock.CreateDbSetMock(new[] { _patient }).Object;

            _availabillity = AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Wednesday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0), new TimeOnly(16, 0), _users);
            _availabillities = DbSetMock.CreateDbSetMock(new[] { _availabillity }).Object;

            _visitList = new List<Visit>()
            {
                VisitFactory.CreateVisit(1, 1, 1, new DateTime(2023, 2, 1, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users,_patients,_rooms,_availabillities),
                VisitFactory.CreateVisit(1, 1, 1, new DateTime(2023, 2, 1, 11, 0, 0), new TimeSpan(1, 0, 0), false, _users,_patients,_rooms,_availabillities),
                VisitFactory.CreateVisit(1, 1, 1, new DateTime(2023, 2, 1, 13, 0, 0), new TimeSpan(1, 0, 0), false, _users,_patients,_rooms,_availabillities),
                VisitFactory.CreateVisit(1, 1, 1, new DateTime(2023, 2, 1, 15, 0, 0), new TimeSpan(1, 0, 0), false, _users,_patients,_rooms,_availabillities),
            };
            _visits = _visitList.AsQueryable();
        }
        [Test]
        public void ShouldPaginateCorrectly()
        {
            Assert.That(_visits.Page(2, 2), Is.EquivalentTo(_visits.Skip(2).Take(2)));
            Assert.That(_visits.Page(1, 2), Is.EquivalentTo(_visits.Take(2)));
            Assert.That(_visits.Page(3, 2), Is.EquivalentTo(_visits.Skip(4)));
        }
        [TestCase(0, 1)]
        [TestCase(0, -1)]
        [TestCase(1, 0)]
        [TestCase(1, -20)]
        public void ShouldFailOnInvalidPagination(int page, int pageSize)
        {
            Assert.Throws<InvalidPaginationException>(() => _visits.Page(page, pageSize));
        }
        [Test]
        public void ShouldFilterNotDeletedCorrectly()
        {
            var indexToDelete = 1;
            _visitList[indexToDelete].MarkAsDeleted();
            Assert.That(_visits.NotDeleted(), Is.EquivalentTo(_visits.Except(new[] { _visitList[indexToDelete] })));
        }
        [Test]
        public void ShouldFilterDeletedCorrectly()
        {
            var indexToDelete = 1;
            _visitList[indexToDelete].MarkAsDeleted();
            Assert.That(_visits.Deleted(), Is.EquivalentTo(new[] { _visitList[indexToDelete] }));
        }
        [Test]
        public void ShouldOrderByCreateDateCorrectly()
        {
            Assert.That(_visits.OrderByCreate(), Is.EquivalentTo(_visitList));
        }
        [Test]
        public void ShouldOrderByDateCorrectly()
        {
            Assert.That(_visits.OrderByCreate(), Is.EquivalentTo(_visitList));
        }
    }
}
