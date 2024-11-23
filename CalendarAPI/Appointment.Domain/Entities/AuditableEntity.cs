using System;

namespace Appointment.Domain.Entities
{
    public abstract class AuditableEntity
    {
        public DateTime Created { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? Deleted { get; set; }
        public string? DeletedBy { get; set; }
        public AuditableEntity()
        {
            Created = DateTime.UtcNow;
        }
    }
}

