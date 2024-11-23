using Appointment.Domain.Entities;
using Appointment.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Data.Configurations
{
    internal class VisitConfiguration : BaseConfiguration<Visit>
    {
        internal override void ConfigureProperties(EntityTypeBuilder<Visit> builder)
        {
            builder.Property(x => x.Id).IsRequired();
        }

        internal override void ConfigureRelations(EntityTypeBuilder<Visit> builder)
        {
            builder
                .HasOne<Patient>(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId);
            builder
                .HasOne<User>(v => v.Therapist)
                .WithMany(u => u.Visits)
                .HasForeignKey(v => v.TherapistId);
            builder
                .HasOne<Room>(v => v.Room)
                .WithMany(r => r.Visits)
                .HasForeignKey(v => v.RoomId);

        }
    }
}
