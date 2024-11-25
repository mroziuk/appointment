# Medical Appointment Booking System

## Project Overview

The Medical Appointment Booking System is an application designed to manage patient appointments.
It provides functionality for user registration, login, adding patients, managing doctor availability, and scheduling appointments.
The system features an intuitive calendar view, making appointment management easier for medical facilities.

## Main Features

- **User registration and login**
- **Patient management**
- **Doctor availability management**
- **Medical appointment booking**
- **Calendar view for appointment management**
- **Automated testing** (unit, integration, end-to-end)

## Technologies and Tools

- **Backend:** .NET 6+
- **Database:** SQL Server / Azure SQL
- **CI/CD:** Azure DevOps
- **Testing:** 
  - Unit and integration tests (NUnit, Xunit)
  - End-to-end tests using Playwright
  - TestContainers
- **Static Code Analysis:** SonarCloud (improves code security, readability, and maintainability)
- **Hosting:** Azure App Services

## System Requirements

- .NET SDK 6.0 or higher
- SQL Server or Azure SQL
- Azure DevOps account (optional for full CI/CD automation)

## Class diagram
```mermaid
classDiagram
	class AuditableEntity{
		<<Abstract>>
	    +Created : DateTime
	    +CreatedBy: string?
	    +LastModified : DateTime?
	    +LastModifiedBy :string? 
	    +Deleted : DateTime?
	    +DeletedBy : string?
	    +AuditableEntity()
	}
	class Room{
		+Id : int
		+Name : string 
		+Visits: List<Visit>
		+markAsDeleted() : void
		+Update(): void
		+Validate(): void
		+Present(): RoomInfoDto
	}
	class Patient{
		+Id : int
		+FirstName: string
		+LastName: string
		+Email: string?
		+Phone: string?
		+IsNew: bool
		+IsRecuring: bool
		+Visits: List<Visit>
		+markAsDeleted() : void
		+Update(): void
		+Validate(): void
		+Present(): PatientInfoDto
	}
	class User{
		+Id: int
		+Login: string
		+Email: string
		+FirstName: string
		+LastName: string
		+Phone: string?
		+PasswordHash: string
		+DateOfBirth: DateOnly?
		+Role : string
		+ResetToken: string
		+Visists: List<Visit>
		+Availabilities: List<Availability>
		+RefreshTokens: ICollection<RefreshToken>
		+User()
		+Update(UpdateUserDto) : void
		+Validate() : void
		-ValidateSuperAdminRole() : void
		-ValidateAdminRole() : void
		-IsLastAdmin() : bool
		+Present() : UserInfoDto
		+Activate() : void
		+MarkAsDeleted()
	}
	class Role{
		+IsValidForRegistration(string) : bool
		+IsValid(string) : bool
	}
	class Availability{
		+Id: int
		+User: User?
		+UserId: int
		+DayOfWeek: DayOfWeek
		+Start: TimeOnly
		+End: TimeOnly
		+ActiveFrom: DateOnly
		+ActiveTo: DateOnly
		+Availabilyty(AddAvailabilityDto)
		+Update(UpdateAvailabilityDto) : void
		+Present() : AvailabilytyInfoDto
		+MarkAsDeleted() : void
	}
	class Visit{
		+Id: int
		+User: Therapist
		+UserId: int
		+Patient: Patient
		+PatientId: int
		+Room: Room
		+RoomId: int
		+Date: DateTime
		+DateTo: DateTime
		+Duration: TimeSpan
		+IsFirstVisit: bool
		+IsRecuring: bool
		+Visit()
		+Present() : VisitInfoDto
		+MarkAsDeleted() : void
		+Update(UpdateVisitDto) : void 
	}

	AuditableEntity <|-- User
	AuditableEntity <|-- Room
	AuditableEntity <|-- Patient
	AuditableEntity <|-- Availability
	AuditableEntity <|-- Visit
	Role *-- User
```

## Database structure
```mermaid
classDiagram
	class Room{
	}
	class Patient{
	}
	class User{
	}
	class Availability{
	}
	class Visit{
	}
	User "1" --> "many" Visit
	User "1" --> "many" Availability
	Room "1" --> "many" Visit
	Patient "1" --> "many" Visit
```
