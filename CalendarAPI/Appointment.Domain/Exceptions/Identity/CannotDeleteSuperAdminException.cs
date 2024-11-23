namespace Appointment.Domain.Exceptions.Identity;
public class CannotDeleteSuperAdminException : DomainException
{
    public CannotDeleteSuperAdminException() : base("Cannot delete super admin") { }
}
