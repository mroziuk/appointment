using Appointment.Common;
using Appointment.Domain.DTO.Patient;
using Appointment.Domain.Entities;
using Appointment.Domain.Exceptions;
using Appointment.Domain.Exceptions.Patient;
using Appointment.Domain.Exceptions.Room;
using Appointment.Tests.Unit.ObjectMothers;
using Appointment.Tests.Unit.Utils;
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
    internal class PatientTests
    {
        [Test]
        public void ShouldBeCreatedCorrectly()
        {
            // assign
            var patient = PatientFactory.CreatePatient("Andrzej", "Kowalski", "333999222");
            Assert.Multiple(() =>
            {
                // act &assert
                Assert.That(patient.FirstName, Is.EqualTo("Andrzej"));
                Assert.That(patient.LastName, Is.EqualTo("Kowalski"));
                Assert.That(patient.Phone, Is.EqualTo("333999222"));
                Assert.That(patient.Created, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
                Assert.That(patient.Deleted, Is.Null);
            });
        }
        [Test]
        public void ShouldFailOnEmptyFirstName()
        {
            Assert.Throws<EmptyFirstNameException>(delegate
            {
                PatientFactory.CreatePatient("", "Kowalski", "123123123");
            });
        }
        [Test]
        public void ShouldFailOnEmptyLastName()
        {
            Assert.Throws<EmptyLastNameException>(delegate
            {
                PatientFactory.CreatePatient("Andrzej", "", "123123123");
            });
        }
        [Test]
        public void ShouldFailOnEmptyPhone()
        {
            Assert.Throws<EmptyPhoneNumberException>(delegate
            {
                PatientFactory.CreatePatient("Andrzej", "Kowalski", "");
            });
        }
        [Test]
        public void ShouldFailOnInvalidPhone()
        {
            Assert.Throws<InvalidPhoneNumberException>(delegate
            {
                PatientFactory.CreatePatient("Andrzej", "Kowalski", "numer");
            });
        }
        [Test]
        public void ShouldFailOnPatientAlradyExist()
        {
            var patient = PatientFactory.CreatePatient("Andrzej", "Kowalski", "000333000",null, 1);
            var mock = DbSetMock.CreateDbSetMock(new[] { patient }).Object;
            Assert.Throws<PatientAlreadyExistException>(delegate
            {
                PatientFactory.CreatePatient("Andrzej", "Kowalski", "000333000",mock, 2);
            });
        }
        [Test]
        public void ShouldFailOnFirstNameTooLong()
        {
            Assert.Throws<FirstNameTooLongException>(delegate
            {
                PatientFactory.CreatePatient(StringUtils.string129chars, "Kowalski", "000333000");
            });
        }
        [Test]
        public void ShouldFailOnLastNameTooLong()
        {
            Assert.Throws<LastNameTooLongException>(delegate
            {
                PatientFactory.CreatePatient("Andrzej", StringUtils.string129chars, "000333000");
            });
        }

    }
    [TestFixture]
    public class PatientUpdateTests
    {
        private Patient _patient;
        [SetUp]
        public void Setup()
        {
            _patient = PatientFactory.CreatePatient("Andrzej", "Kowalski", "333999222");
        }
        [Test]
        public void ShouldBeUpdatedCorrectly()
        {
            var newFirstName = "Adam";
            var newLastName = "Małysz";
            var newPhone = "111222333";
            var newEmail = "adam.malysz@g.gmail.com";
            var dto = new UpdatePatientDto()
            {
                FirstName = newFirstName,
                LastName = newLastName,
                Phone = newPhone,
                Email = newEmail,
            };
            _patient.Update(dto);
            Assert.Multiple(() =>
            {
                Assert.That(_patient.FirstName, Is.EqualTo(newFirstName));
                Assert.That(_patient.LastName, Is.EqualTo(newLastName));
                Assert.That(_patient.Phone, Is.EqualTo(newPhone));
                Assert.That(_patient.Email, Is.EqualTo(newEmail));
            });
        }
        [Test]
        public void ShoulBeUpdatedCorrectlyWithFirstNameOnly()
        {
            var newFirstName = "Adam";
            var dto = new UpdatePatientDto()
            {
                FirstName = newFirstName,
                LastName = PropertyUpdater<string>.Keep,
                Phone = PropertyUpdater<string>.Keep,
                Email = PropertyUpdater<string>.Keep,
            };
            _patient.Update(dto);
            Assert.Multiple(() =>
            {
                Assert.That(_patient.FirstName, Is.EqualTo(newFirstName));
                Assert.That(_patient.LastName, Is.EqualTo("Kowalski"));
                Assert.That(_patient.Phone, Is.EqualTo("333999222"));
                Assert.That(_patient.Email, Is.EqualTo(""));
            });
        }
        [Test]
        public void ShoulBeUpdatedCorrectlyWithLastNameOnly()
        {
            var newLastName = "Małysz";
            var dto = new UpdatePatientDto()
            {
                FirstName = PropertyUpdater<string>.Keep,
                LastName = newLastName,
                Phone = PropertyUpdater<string>.Keep,
                Email = PropertyUpdater<string>.Keep,
            };
            _patient.Update(dto);

            Assert.Multiple(() =>
            {
                Assert.That(_patient.FirstName, Is.EqualTo("Andrzej"));
                Assert.That(_patient.LastName, Is.EqualTo(newLastName));
                Assert.That(_patient.Phone, Is.EqualTo("333999222"));
                Assert.That(_patient.Email, Is.EqualTo(""));
            });
        }
        [Test]
        public void ShoulBeUpdatedCorrectlyWithPhoneOnly()
        {
            var newPhone = "111222333";
            var dto = new UpdatePatientDto()
            {
                FirstName = PropertyUpdater<string>.Keep,
                LastName = PropertyUpdater<string>.Keep,
                Phone = newPhone,
                Email = PropertyUpdater<string>.Keep,
            };
            _patient.Update(dto);
            Assert.Multiple(() =>
            {
                Assert.That(_patient.FirstName, Is.EqualTo("Andrzej"));
                Assert.That(_patient.LastName, Is.EqualTo("Kowalski"));
                Assert.That(_patient.Phone, Is.EqualTo(newPhone));
                Assert.That(_patient.Email, Is.EqualTo(""));
            });
        }
        [Test]
        public void ShouldFailOnEmptyFirstName()
        {
            var dto = new UpdatePatientDto()
            {
                FirstName = "",
                LastName = PropertyUpdater<string>.Keep,
                Phone = PropertyUpdater<string>.Keep,
                Email = PropertyUpdater<string>.Keep,
            };
            Assert.Throws<EmptyFirstNameException>(delegate
            {
                _patient.Update(dto);
            });
        }
    }
    [TestFixture]
    public class ParientDeleteTests
    {

        [Test]
        public void ShouldBeDeletedCorrectly()
        {
            //var patient = PatientFactory.CreatePatient("Andrzej", "Kowalski", "333999222");
            //patient.MarkAsDeleted();
            //Assert.That(patient.Deleted, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
        [Test]
        public void ShouldBeDeletedCorrectlyWIthVisits()
        {
            //Assert.Fail();
        }
    }
    [TestFixture]
    public class PatientSpecificationTests
    {
        private List<Patient> _patientList;
        private IQueryable<Patient> _patients;
        [SetUp]
        public void Setup()
        {
            _patientList = new List<Patient>()
            {
                PatientFactory.CreatePatient("Adam", "małysz","333000333"),
                PatientFactory.CreatePatient("Andrzej", "Kowalski","111000111"),
                PatientFactory.CreatePatient("Robert", "Baranowski","222000222"),
                PatientFactory.CreatePatient("Robert", "Lewandowski","222000222"),
                PatientFactory.CreatePatient("Sebastian", "Rejent","444000444"),
            };
            _patients = _patientList.AsQueryable();
        }
        [Test]
        public void ShouldPaginateCorrectly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_patients.Page(2, 2), Is.EquivalentTo(_patients.Skip(2).Take(2)));
                Assert.That(_patients.Page(1, 2), Is.EquivalentTo(_patients.Take(2)));
                Assert.That(_patients.Page(3, 2), Is.EquivalentTo(_patients.Skip(4)));
            });
        }
        [TestCase(0, 1)]
        [TestCase(0, -1)]
        [TestCase(1, 0)]
        [TestCase(1, -20)]
        public void ShouldFailOnInvalidPagination(int page, int pageSize)
        {
            Assert.Throws<InvalidPaginationException>(delegate
            {
                _patients.Page(page, pageSize);
            });
        }
        [Test]
        public void ShouldFilterNotDeletedCorrectly()
        {
            var indexToDelete = 1;
            _patientList[indexToDelete].MarkAsDeleted();
            Assert.That(_patients.NotDeleted(), Is.EquivalentTo(_patients.Except(new[] { _patientList[indexToDelete] })));
        }
        [Test]
        public void ShouldFilterDeletedCorrectly()
        {
            var indexToDelete = 1;
            _patientList[indexToDelete].MarkAsDeleted();
            Assert.That(_patients.Deleted(), Is.EquivalentTo(new[] { _patientList[indexToDelete] }));
        }
        [Test]
        public void ShouldOrderByCreateDateCorrectly()
        {
            Assert.That(_patients.OrderByCreateDate(), Is.EqualTo(_patientList).AsCollection);
        }
        [Test]
        public void ShouldOrderByCreateDateDescendingCorrectly()
        {
            Assert.That(_patients.OrderByCreateDate(true), Is.EqualTo(_patientList.OrderByDescending(i => i.Created)).AsCollection);
        }
        [Test]
        public void ShouldOrderByNameCorrectly()
        {
            Assert.That(_patients.OrderByName(), Is.EqualTo(_patientList).AsCollection);
        }
        [Test]
        public void ShouldOrderByNameDescendingCorrectly()
        {
            _patientList.Reverse();
            Assert.That(_patients.OrderByName(true), Is.EqualTo(_patientList).AsCollection);
        }

    }
}
