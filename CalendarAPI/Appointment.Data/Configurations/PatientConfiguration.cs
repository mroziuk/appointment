using Appointment.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Data.Configurations
{
    internal class PatientConfiguration : BaseConfiguration<Patient>
    {
        internal override void ConfigureProperties(EntityTypeBuilder<Patient> builder)
        {
            builder
                .Property(x => x.Id)
                .IsRequired();
        }

        internal override void ConfigureRelations(EntityTypeBuilder<Patient> builder)
        {
            builder
                .HasMany<Visit>(p => p.Visits)
                .WithOne(v => v.Patient)
                .HasForeignKey(v => v.PatientId);
        }
    }
}
