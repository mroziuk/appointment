using Appointment.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Data.Configurations
{
    internal class RoomConfiguration : BaseConfiguration<Room>
    {
        internal override void ConfigureProperties(EntityTypeBuilder<Room> builder)
        {
            builder.Property(x => x.Id).IsRequired();
        }

        internal override void ConfigureRelations(EntityTypeBuilder<Room> builder)
        {
            builder
                .HasMany<Visit>(r => r.Visits)
                .WithOne(v => v.Room)
                .HasForeignKey(v => v.RoomId);
        }
    }
}
