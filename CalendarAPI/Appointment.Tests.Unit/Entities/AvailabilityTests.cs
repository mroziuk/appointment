using Appointment.Common;
using Appointment.Domain.DTO.Availability;
using Appointment.Domain.Entities;
using Appointment.Domain.Entities.Identity;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.Availability;
using Appointment.Domain.Exceptions.Visit;
using Appointment.Tests.Unit.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Appointment.Tests.Unit.ObjectMothers.EntityFactory;

namespace Appointment.Tests.Unit.Entities
{
    [TestFixture]
    internal class AvailabilityTests
    {
        private User _user;
        private DbSet<User> _users;
        [SetUp]
        public void SetUp()
        {
            _user = UserFactory.CreateUser("login", "email@gmail.com", "firstName", "lastName", "123000111");
            _users = DbSetMock.CreateDbSetMock(new List<User> { _user }).Object;
        }
        [Test]
        public void ShouldBeCreatedCorrectly()
        {
            // assign
            var availability = AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023,1,1), new DateOnly(2023,12,31),new TimeOnly(8,0,0),new TimeOnly(16,0,0),_users);
            Assert.Multiple(() =>
            {
                // act & assert
                Assert.That(availability.UserId, Is.EqualTo(_user.Id));
                Assert.That(availability.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
                Assert.That(availability.ActiveFrom, Is.EqualTo(new DateOnly(2023, 1, 1)));
                Assert.That(availability.ActiveTo, Is.EqualTo(new DateOnly(2023, 12, 31)));
                Assert.That(availability.Start, Is.EqualTo(new TimeOnly(8, 0, 0)));
                Assert.That(availability.End, Is.EqualTo(new TimeOnly(16, 0, 0)));
                Assert.That(availability.Created, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
                Assert.That(availability.Deleted, Is.Null);
            });
        }
        [Test]
        public void ShouldFailOnInvalidUserId()
        {
            Assert.Throws<NoAvailabilityUserException>(delegate
            {
                AvailabilityFactory.CreateAvailability(-1, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users);
            });
        }
        [Test]
        public void ShouldFailOnDateEndBeforeStart()
        {
            Assert.Throws<DateOnlyEndBeforeStartException>(delegate
            {
                AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2022, 12, 31), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users);
            });
        }
        [Test]
        public void ShouldFailOnTimeEndBeforeStart()
        {
            Assert.Throws<TimeOnlyEndBeforeStartException>(delegate
            {
                AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(16, 0, 0), new TimeOnly(8, 0, 0), _users);
            });
        }
    }
    [TestFixture]
    public class  AvailabilityUpdateTests
    {
        private User _user;
        private DbSet<User> _users;
        [SetUp]
        public void SetUp()
        {
            _user = UserFactory.CreateUser("login", "user@gmail.com", "firstName", "lastName", "123000111");
            _users = DbSetMock.CreateDbSetMock(new List<User> { _user }).Object;
        }
        [Test]
        public void ShouldBeUpdatedCorrectly()
        {
            // assign
            var availability = 
                AvailabilityFactory
                .CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users);
            // act
            UpdateAvailabilityDto dto = new()
            {
                DayOfWeek = (int)DayOfWeek.Tuesday,
                Start = new TimeOnly(9, 0, 0),
                End = new TimeOnly(17, 0, 0),
                ActiveFrom = new DateOnly(2023, 2, 1),
                ActiveTo = new DateOnly(2023, 6, 30),
                UserId = _user.Id
            };
            availability.Update(dto, _users, null);
            Assert.Multiple(() =>
            {
                // assert
                Assert.That(availability.UserId, Is.EqualTo(_user.Id));
                Assert.That(availability.DayOfWeek, Is.EqualTo(DayOfWeek.Tuesday));
                Assert.That(availability.ActiveFrom, Is.EqualTo(new DateOnly(2023, 2, 1)));
                Assert.That(availability.ActiveTo, Is.EqualTo(new DateOnly(2023, 6, 30)));
                Assert.That(availability.Start, Is.EqualTo(new TimeOnly(9, 0, 0)));
                Assert.That(availability.End, Is.EqualTo(new TimeOnly(17, 0, 0)));
                Assert.That(availability.LastModified, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            });
        }
        [Test]
        public void ShouldBeUpdated_WithStartOnly()
        {
            var availability = AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users);
            UpdateAvailabilityDto dto = new()
            {
                Start = new TimeOnly(9, 0, 0),
                UserId = _user.Id,
                DayOfWeek = PropertyUpdater<int>.Keep,
                ActiveFrom = PropertyUpdater<DateOnly>.Keep,
                ActiveTo = PropertyUpdater<DateOnly>.Keep,
                End = PropertyUpdater<TimeOnly>.Keep
            };
            availability.Update(dto, _users, null);
            Assert.Multiple(() =>
            {
                Assert.That(availability.Start, Is.EqualTo(new TimeOnly(9, 0, 0)));
                Assert.That(availability.End, Is.EqualTo(new TimeOnly(16, 0, 0)));
                Assert.That(availability.LastModified, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
                Assert.That(availability.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
                Assert.That(availability.ActiveFrom, Is.EqualTo(new DateOnly(2023, 1, 1)));
                Assert.That(availability.ActiveTo, Is.EqualTo(new DateOnly(2023, 12, 31)));
            });
        }
        [Test]
        public void ShouldByUpdated_WithDayOfWeekOnly()
        {
            var availability = AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users);
            UpdateAvailabilityDto dto = new()
            {
                DayOfWeek = (int)DayOfWeek.Friday,
                UserId = _user.Id,
                ActiveFrom = PropertyUpdater<DateOnly>.Keep,
                ActiveTo = PropertyUpdater<DateOnly>.Keep,
                End = PropertyUpdater<TimeOnly>.Keep,
                Start = PropertyUpdater<TimeOnly>.Keep
            };
            availability.Update(dto, _users, null);
            Assert.Multiple(() =>
            {
                Assert.That(availability.DayOfWeek, Is.EqualTo(DayOfWeek.Friday));
                Assert.That(availability.Start, Is.EqualTo(new TimeOnly(8, 0, 0)));
                Assert.That(availability.End, Is.EqualTo(new TimeOnly(16, 0, 0)));
                Assert.That(availability.LastModified, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
                Assert.That(availability.ActiveFrom, Is.EqualTo(new DateOnly(2023, 1, 1)));
                Assert.That(availability.ActiveTo, Is.EqualTo(new DateOnly(2023, 12, 31)));
            });
        }
        [Test]
        public void ShouldFailOnUserNotExist()
        {
            var availability = AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users);
            UpdateAvailabilityDto dto = new()
            {
                UserId = 2,
                DayOfWeek = PropertyUpdater<int>.Keep,
                ActiveFrom = PropertyUpdater<DateOnly>.Keep,
                ActiveTo = PropertyUpdater<DateOnly>.Keep,
                End = PropertyUpdater<TimeOnly>.Keep,
                Start = PropertyUpdater<TimeOnly>.Keep
            };
            Assert.Throws<NoAvailabilityUserException>(delegate
            {
                availability.Update(dto, _users, null);
            });
        }
        [Test]
        public void ShouldFailOnOverlap()
        {
            var availability =
                AvailabilityFactory
                .CreateAvailability(
                    _user.Id, DayOfWeek.Monday,
                    new DateOnly(2023, 1, 1),
                    new DateOnly(2023, 12, 31), 
                    new TimeOnly(11, 0, 0),
                    new TimeOnly(17, 0, 0),
                    _users,
                    null,
                    1
                );
            var availability2 = 
                AvailabilityFactory
                .CreateAvailability(
                    _user.Id,
                    DayOfWeek.Monday,
                    new DateOnly(2023, 1, 1), 
                    new DateOnly(2023, 12, 31), 
                    new TimeOnly(8, 0, 0), 
                    new TimeOnly(10, 0, 0), 
                    _users,
                    null,
                    2
                );
            var availabilities = DbSetMock.CreateDbSetMock(new List<Availabillity> { availability2 }).Object;
            UpdateAvailabilityDto dto = new()
            {
                UserId = PropertyUpdater<int>.Keep,
                DayOfWeek = PropertyUpdater<int>.Keep,
                ActiveFrom = PropertyUpdater<DateOnly>.Keep,
                ActiveTo = PropertyUpdater<DateOnly>.Keep,
                Start = new TimeOnly(9, 0, 0),
                End = PropertyUpdater<TimeOnly>.Keep,

            };
            Assert.Throws<AvailabilityOverlapException>(delegate
            {
                availability.Update(dto, _users, availabilities);
            });
        }
    }

    [TestFixture]
    public class AvailabilityDeleteTests
    {
        private User _user;
        private DbSet<User> _users;
        [SetUp]
        public void SetUp()
        {
            _user = UserFactory.CreateUser("login", "andrzej.kowalski@gmail.com", "Andrzej", "Kowalski", "123000111");
            _users = DbSetMock.CreateDbSetMock(new List<User> { _user }).Object;
        }
        [Test]
        public void ShouldBeDeletedCorrectly()
        {
           var availability = AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users);
            availability.MarkAsDeleted();
            Assert.That(availability.Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
    }
    [TestFixture]
    public class AvailabillitySpecificationTests
    {
        private User _user;
        private User _user2;
        private DbSet<User> _users;
        private List<Availabillity> _availabilityList;
        private IQueryable<Availabillity> _availabillities;
        [SetUp]
        public void SetUp()
        {
            _user = UserFactory.CreateUser("akowalski", "andrzej.kowalski@gmail.com", "Andrzej", "Kowalski", "123000111","user", null,1);
            _user2 = UserFactory.CreateUser("jkowalski","jan.kowalski@gmail.com", "Jan", "Kowalski", "123000111","user", null,2);
            _users = DbSetMock.CreateDbSetMock(new List<User> { _user, _user2 }).Object;
            _availabilityList = new()
            {
                AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Monday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users),
                AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Tuesday, new DateOnly(2023, 1, 1), new DateOnly(2023, 12, 31), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users),
                AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Wednesday, new DateOnly(2023, 2, 1), new DateOnly(2023, 6, 30), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users),
                AvailabilityFactory.CreateAvailability(_user.Id, DayOfWeek.Thursday, new DateOnly(2023, 2, 1), new DateOnly(2023, 6, 30), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users),
                AvailabilityFactory.CreateAvailability(_user2.Id, DayOfWeek.Friday, new DateOnly(2023, 2, 1), new DateOnly(2023, 6, 30), new TimeOnly(8, 0, 0), new TimeOnly(16, 0, 0), _users),
            };
            _availabillities = _availabilityList.AsQueryable();
        }
        [Test]
        public void ShouldPaginateCorrectly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_availabillities.Page(2, 2), Is.EquivalentTo(_availabillities.Skip(2).Take(2)));
                Assert.That(_availabillities.Page(1, 2), Is.EquivalentTo(_availabillities.Take(2)));
                Assert.That(_availabillities.Page(3, 2), Is.EquivalentTo(_availabillities.Skip(4)));
            });
        }
        [TestCase(0,1)]
        [TestCase(1,0)]
        [TestCase(-1,1)]
        [TestCase(1,-1)]
        [TestCase(-1,-1)]
        public void ShouldFailOnInvalidPagination(int page, int pageSize)
        {
            Assert.Throws<InvalidPaginationException>(delegate
            {
                _availabillities.Page(page, pageSize);
            });
        }
        [Test]
        public void ShouldFilterByUserId()
        {
            Assert.That(_availabillities.WithUser(_user.Id), Is.EquivalentTo(_availabilityList.Take(4)));
        }
        [Test]
        public void ShouldFilterByDayOfWeek()
        {
            Assert.That(_availabillities.FromDayOfWeek((int)DayOfWeek.Monday), Is.EquivalentTo(_availabilityList.Take(1)));
        }
        [Test]
        public void ShouldFilterNotDeleted()
        {
            _availabilityList[0].MarkAsDeleted();
            Assert.That(_availabillities.NotDeleted(), Is.EquivalentTo(_availabilityList.Skip(1)));
        }
        [Test]
        public void ShouldFilterFromDate()
        {
            Assert.That(_availabillities.FromDate(new DateOnly(2023, 2, 1)), Is.EquivalentTo(_availabilityList.Skip(2)));
        }
        [Test]
        public void ShouldFilterToDate()
        {
            Assert.That(_availabillities.ToDate(new DateOnly(2023, 7, 1)), Is.EquivalentTo(_availabilityList.TakeLast(3)));
        }
        [Test]
        public void ShouldFilterOverlaping()
        {

        }
    }
}
