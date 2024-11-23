namespace Appointment.Domain.Exceptions.Identity;

public class CannotDeleteLastAdminException : DomainException
{
    public CannotDeleteLastAdminException() : base("Cannot delete last admin") { }
}
