using System.Text.Json.Serialization;

namespace Appointment.Domain.DTO.Visit;

public record GetVisitDto
{
    [JsonConstructor]
    public GetVisitDto(int id, int therapistId, int patientId, int roomId, DateTime startDate, DateTime endDate, bool isFirstVisit, bool isRecurring)
    {
        Id = id;
        TherapistId = therapistId;
        PatientId = patientId;
        RoomId = roomId;
        StartDate = startDate;
        EndDate = endDate;
        IsFirstVisit = isFirstVisit;
        IsRecurring = isRecurring;
    }
    public GetVisitDto(int id, int therapistId, int patientId, int roomId, DateTime date, TimeSpan duration, bool isFirstVisit, bool isRecurring)
    {
        Id = id;
        TherapistId = therapistId;
        PatientId = patientId;
        RoomId = roomId;
        StartDate = date;
        EndDate = date.Add(duration);
        IsFirstVisit = isFirstVisit;
        IsRecurring = isRecurring;
    }
    public GetVisitDto(Entities.Visit visit)
    {
        Id = visit.Id;
        TherapistId = visit.TherapistId;
        PatientId = visit.PatientId;
        RoomId = visit.RoomId;
        StartDate = visit.Date;
        EndDate = visit.DateTo;
        IsFirstVisit = visit.IsFirstVisit;
        IsRecurring = visit.IsRecurring;
    }
    public GetVisitDto() { }
    public int Id { get; set; }
    public int TherapistId { get; set; }
    public int PatientId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsFirstVisit { get; set; }
    public bool IsRecurring { get; set; }

}
