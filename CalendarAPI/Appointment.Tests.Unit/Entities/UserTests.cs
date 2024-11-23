using Appointment.Common;
using Appointment.Domain.DTO;
using Appointment.Domain.Entities;
using Appointment.Domain.Entities.Identity;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.User;
using Appointment.Tests.Unit.ObjectMothers;
using Appointment.Tests.Unit.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;
using static Appointment.Tests.Unit.ObjectMothers.EntityFactory;

namespace Appointment.Tests.Unit.Entities
{
    [TestFixture]
    internal class UserTests
    {
        [Test]
        public void ShouldBeCreatedCorrectly()
        {
            var user = UserFactory.CreateUser("user1", "user1@gmail.com", "firstName", "lastName", "999333888");

            Assert.Multiple(() =>
            {
                Assert.That(user.Login, Is.EqualTo("user1"));
                Assert.That(user.Email, Is.EqualTo("user1@gmail.com"));
                Assert.That(user.FirstName, Is.EqualTo("firstName"));
                Assert.That(user.LastName, Is.EqualTo("lastName"));
                Assert.That(user.Created, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
                Assert.That(user.Deleted, Is.Null);
                Assert.That(user.IsActive, Is.False);
            });
        }
        [Test]
        public void CreateUser_WhenEmailExist_ShouldThrowException()
        {
            var user = UserFactory.CreateUser("user1", "user1@gmail.com", "firstName", "lastName", "999333888");
            var users = DbSetMock.CreateDbSetMock(new List<User>() { user }).Object;

            Assert.Throws<EmailAlreadyInUseException>(delegate
            {
                UserFactory.CreateUser("user2", "user1@gmail.com", "firstName1", "lastName1", "222444111", "user", users);
            });
        }
        [Test]
        [TestCase("login@@gmail.com")]
        [TestCase("logingmail.com")]
        [TestCase("login@gmail")]
        [TestCase("login @gmail.com.pl")]
        [TestCase("logingma@gm@ail.com")]
        public void CreateUser_WhenEmailInvalid_ShouldThrowException(string login)
        {
            Assert.Throws<InvalidEmailException>(delegate
            {
                UserFactory.CreateUser("login", login, "firstName1", "lastName1", "222444111");
            });
        }
        [Test]
        public void CreateUser_WhenEmailEmpty_ShouldThrowException()
        {
            Assert.Throws<EmptyEmailException>(delegate
            {
                UserFactory.CreateUser("login", "", "firstName1", "lastName1", "222444111");
            });
        }
        [Test]
        public void CreateUser_WhenLoginEmpty_ShouldThrowException()
        {
            Assert.Throws<EmptyLoginException>(delegate
            {
                UserFactory.CreateUser("", "user222@gmail.com", "firstName1", "lastName1", "222444111");
            });
        }
        [Test]
        public void CreateUser_WhenLoginTooLong_ShouldThrowException()
        {
            Assert.Throws<LoginTooLongException>(delegate
            {
                UserFactory.CreateUser("129_chars_ygol6Nc2d6BIf7dt5pqKWS1pTbgwZnTgagkt88v84kVPD37THVQ6scWPYa2psdpZ7mv391QcVovFRXRNa9NrGznWUO59myf9XZzuUYW5xNJM7foKSfPq0DE", "user222@gmail.com", "firstName1", "lastName1", "222444111");
            });
        }
        [Test]
        public void CreateUser_WhenLoginExist_ShouldThrowException()
        {
            var user = UserFactory.CreateUser("user1", "user1@gmail.com", "firstName", "lastName", "999333888");
            var users = DbSetMock.CreateDbSetMock(new List<User>() { user }).Object;

            Assert.Throws<LoginAlreadyExistException>(delegate
            {
                UserFactory.CreateUser("user1", "user222@gmail.com", "firstName1", "lastName1", "222444111","user", users);
            });
        }
        [Test]
        public void CreateUser_ShouldTrimEmail()
        {
            Assert.That(UserFactory.CreateUser("login", "    login@gmail.com     ", "firstName1", "lastName1", "222444111").Email, Is.EqualTo("login@gmail.com"));
        }
        [Test]
        public void CreateUser_ShouldTrimFirstName()
        {
            Assert.That(UserFactory.CreateUser("login", "login@gmail.com", "      firstName  \n   ", "lastName1", "222444111").FirstName, Is.EqualTo("firstName"));
        }
        [Test]
        public void CreateUser_ShouldTrimlastName()
        {
            Assert.That(UserFactory.CreateUser("login", "login@gmail.com", "firstName1", "     lastName      ", "222444111").LastName, Is.EqualTo("lastName"));
        }
        [Test]
        public void CreateUser_ShouldTrimLogin()
        {
            Assert.That(UserFactory.CreateUser("      login      ", "login@gmail.com", "firstName", "lastName", "222444111").Login, Is.EqualTo("login")); 
        }
        [Test]
        public void CreateUser_ShouldTrimPhoneNumber()
        {
            Assert.That(UserFactory.CreateUser("login", "login@gmail.com", "firstName", "lastName", "   222333111   ").Phone, Is.EqualTo("222333111"));
        }
    }
    [TestFixture]
    public class UserUpdateTests
    {
        private User _user;
        private DbSet<User> _users;
        [SetUp]
        public void Setup()
        {
            _user = UserFactory.CreateUser("user1", "user1@gmail.com", "firstName", "lastName", "999333888");
        }
        [Test]
        public void ShouldBeUpdatedCorrectly()
        {
            _user.Update(new UpdateUserDto()
            {
                Login = "user2",
                FirstName = "firstName2",
                LastName = "lastName2",
                Email = "user.new.email@gmail.com",
                Phone = "111222333",
                Role = "admin",
                IsActive = false
            });
            Assert.That(_user.Login, Is.EqualTo("user2"));
            Assert.That(_user.FirstName, Is.EqualTo("firstName2"));
            Assert.That(_user.LastName, Is.EqualTo("lastName2"));
            Assert.That(_user.Email, Is.EqualTo("user.new.email@gmail.com"));
            Assert.That(_user.Phone, Is.EqualTo("111222333"));
            Assert.That(_user.Role, Is.EqualTo("admin"));
            Assert.That(_user.IsActive, Is.False);
        }
        [Test]
        public void ShouldBeUpdatedCorrectlyWithEmailOnly()
        {
            _user.Update(new UpdateUserDto()
            {
                Login = PropertyUpdater<string>.Keep,
                FirstName = PropertyUpdater<string>.Keep,
                LastName = PropertyUpdater<string>.Keep,
                Email = "user.new.email@gmail.com",
                Phone = PropertyUpdater<string>.Keep,
                Role = PropertyUpdater<string>.Keep,
                IsActive = PropertyUpdater<bool>.Keep,
            });
            Assert.Multiple(() =>
            {
                Assert.That(_user.Login, Is.EqualTo("user1"));
                Assert.That(_user.Email, Is.EqualTo("user.new.email@gmail.com"));
                Assert.That(_user.FirstName, Is.EqualTo("firstName"));
                Assert.That(_user.LastName, Is.EqualTo("lastName"));
                Assert.That(_user.Created, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
                Assert.That(_user.Deleted, Is.Null);
                //TODO: change isActive after activation email will be done
                Assert.That(_user.IsActive, Is.False);
            });
        }
    }   
    [TestFixture]
    public class UserDeleteTests
    {
        private User _user;
        private DbSet<User> _users;
        private Availabillity _availabillity;
        private DbSet<Availabillity> _availabillities;
        [SetUp]
        public void Setup()
        {
            _user = UserFactory.CreateUser("login", "login.abc@gmail.com", "firstName", "lastName", "111222333");
            _users = DbSetMock.CreateDbSetMock(new List<User>() { _user }).Object;

            _availabillity = AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0), new TimeOnly(16, 0), _users);
            _availabillities = DbSetMock.CreateDbSetMock(new List<Availabillity>() { _availabillity }).Object;
        }
        [Test]
        public void ShouldBeDeletedCorrectly()
        {
            _user.MarkAsDeleted();
            Assert.That(_user.Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
        [Test]
        public void ShouldBeDeletedCorrectlyWithAvailabilities()
        {
            _user.Availabillities = _availabillities.ToList();
            _user.MarkAsDeleted();
            Assert.That(_user.Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            Assert.That(_user.Availabillities.First().Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
        [Test]
        public void ShouldBeDeletedCorrectlyWithVisits()
        {

            var patient = PatientFactory.CreatePatient("Jan", "Nowak", "999333222");
            var patients = DbSetMock.CreateDbSetMock(new[] { patient }).Object;
            var room = RoomFactory.CreateRoom("room 1");
            var rooms = DbSetMock.CreateDbSetMock(new[] { room }).Object;
            var visit = VisitFactory.CreateVisit(_user.Id, patient.Id, room.Id, new DateTime(2023, 12, 4, 9, 0, 0), new TimeSpan(1, 0, 0), false, _users, patients, rooms, _availabillities);
            _user.Visits = new() { visit };
            _user.MarkAsDeleted();
            Assert.That(_user.Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            Assert.That(_user.Visits.First().Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
    }
    [TestFixture]
    public class UserSpecificationTests
    {
        private IQueryable<User> _users;
        private List<User> _userList;
        [SetUp]
        public void Setup()
        {
            _userList = new List<User>
            {
                UserFactory.CreateUser("akowalski", "andrzej.kowalski@gmail.com","andrzej", "kowalski","111000111"),
                UserFactory.CreateUser("login2", "email2@gmail.com","firstName2", "lastName2","111000112"),
                UserFactory.CreateUser("login3", "email3@gmail.com","firstName3", "lastName3","111000113"),
                UserFactory.CreateUser("login4", "email4@gmail.com","firstName4", "lastName4","111000114"),
            };
            _users = _userList.AsQueryable();
        }
        [Test]
        public void ShouldPaginateCorrectly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_users.Page(2, 2), Is.EquivalentTo(_users.Skip(2).Take(2)));
                Assert.That(_users.Page(1, 2), Is.EquivalentTo(_users.Take(2)));
                Assert.That(_users.Page(3, 2), Is.EquivalentTo(_users.Skip(4)));
            });
        }
        [TestCase(-1,0)]
        [TestCase(-10,-10)]
        [TestCase(0,1)]
        [TestCase(1,0)]
        public void ShouldFailOnInvalidPagination(int page, int pagesize)
        {
            Assert.Throws<InvalidPaginationException>(delegate
            {
                _users.Page(page, pagesize);
            });
        }
        [Test]
        public void ShouldFilterByIdCorrectly()
        {
            int id = _userList[0].Id;
            Assert.That(_users.WithId(id), Is.EqualTo(_userList[0]));
        }
        [TestCase("andrzej")]
        [TestCase("Andrz")]
        [TestCase("Kowal")]
        [TestCase("k")]
        [TestCase("      kowalski     ")]
        public void ShouldFilterByNameCorrectly(string name)
        {
            Assert.That(_users.WithName(name), Is.EqualTo(new[] { _userList[0] }).AsCollection);
        }
        [TestCase("andrze")]
        [TestCase("kowals")]
        [TestCase("KoWAlsKI")]
        [TestCase("andrzej.kowal")]
        [TestCase("andrzej.kowalski@gmail.com")]
        public void ShouldFilterWithEmailCorrectly(string email)
        {
            Assert.That(_users.WithEmail(email), Is.EqualTo(new[] { _userList[0] }).AsCollection);
        }
        [TestCase("akowalski")]
        [TestCase("kowal")]
        [TestCase("KOWal")]
        [TestCase("AKO")]
        public void ShouldFilterWithLoginCorrectly(string login)
        {
            Assert.That(_users.WithLogin(login), Is.EqualTo(new[] { _userList[0] }).AsCollection);
        }
        [Test]
        public void ShouldFilterNotDeletedCorrectly()
        {
            var indexToDelete = 1;
            _userList[indexToDelete].MarkAsDeleted();
            Assert.That(_users.NotDeleted(), Is.EquivalentTo(_users.Except(new[] { _userList[indexToDelete] })));
        }
        [Test]
        public void ShouldOrderByCreateDateCorrectly()
        {
            Assert.That(_users.OrderByCreateDate(), Is.EqualTo(_userList).AsCollection);
        }
        [Test]
        public void ShouldOrderByCreateDateDescendingCorrectly()
        {
            Assert.That(_users.OrderByCreateDate(true), Is.EqualTo(_userList.OrderByDescending(i => i.Created)).AsCollection);
        }
        [Test]
        public void ShouldOrderByNameCorrectly()
        {
            Assert.That(_users.OrderByName(), Is.EqualTo(_userList).AsCollection);
        }
    }
}
